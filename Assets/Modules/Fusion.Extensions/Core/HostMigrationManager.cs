using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Fusion
{
    [DisallowMultipleComponent]
    public class HostMigrationManager : SimulationBehaviour, IAfterHostMigration
    {
        public struct MigratedTransform
        {
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;
        }

        private readonly List<MigratedTransform> _migratedTransforms = new();

        [SerializeField]
        private float _sendInterval = 1;

        private TickTimer _snapshotTimer;

        public override void FixedUpdateNetwork()
        {
            if (!this.Runner.IsServer)
                return;

            if (_snapshotTimer.ExpiredOrNotRunning(this.Runner))
            {
                this.Runner.PushHostMigrationSnapshot();
                _snapshotTimer = TickTimer.CreateFromSeconds(this.Runner, _sendInterval);
            }
        }

        public void ProcessMigration()
        {
            this.ProcessSpawnedObjects();
            this.ProcessSceneObjects();
        }

        private void ProcessSpawnedObjects()
        {
            foreach (NetworkObject snapshots in this.Runner.GetResumeSnapshotNetworkObjects())
            {
                NetworkObjectTypeId typeId = snapshots.NetworkTypeId;
                if (!typeId.IsValid || !typeId.IsPrefab)
                    continue;

                Debug.Log($"Spawned object migration {snapshots.Name}");
                bool hasTransform = snapshots.TryGetBehaviour(out NetworkTRSP transform);
                Vector3 position = hasTransform ? transform.Data.Position : Vector3.zero;
                Quaternion rotation = hasTransform ? transform.Data.Rotation : Quaternion.identity;

                this.Runner.Spawn(snapshots, position, rotation, PlayerRef.None,
                    onBeforeSpawned: (_, obj) => obj.CopyStateFrom(snapshots)
                );
            }
        }

        private void ProcessSceneObjects()
        {
            HashSet<NetworkObject> migratedObjects = HashSetPool<NetworkObject>.Get();
            
            foreach ((NetworkObject sceneObject, NetworkObjectHeaderPtr snapshot) in
                     this.Runner.GetResumeSnapshotNetworkSceneObjects())
            {
                Debug.Log($"Scene object migration {sceneObject.name} {snapshot.Id}");
                sceneObject.CopyStateFrom(snapshot);
                migratedObjects.Add(sceneObject);

                this.ProcessSceneObjectTransform(sceneObject);
            }
            
            // Destroy all scene objects not migrate
            List<NetworkObject> allObjects = ListPool<NetworkObject>.Get();
            this.Runner.GetAllNetworkObjects(allObjects);
            foreach (NetworkObject obj in allObjects)
                if (obj.NetworkTypeId.IsSceneObject && !migratedObjects.Contains(obj))
                    this.Runner.Despawn(obj);
            
            HashSetPool<NetworkObject>.Release(migratedObjects);
            ListPool<NetworkObject>.Release(allObjects);
        }

        private void ProcessSceneObjectTransform(NetworkObject sceneObject)
        {
            if (sceneObject.TryGetBehaviour(out NetworkTRSP transform))
            {
                NetworkTRSPData transformData = transform.Data;
                _migratedTransforms.Add(new MigratedTransform
                {
                    transform = sceneObject.transform,
                    position = transformData.Position,
                    rotation = transformData.Rotation
                });
            }

            foreach (NetworkObject networkObject in sceneObject.NestedObjects) 
                this.ProcessSceneObjectTransform(networkObject);
        }

        void IAfterHostMigration.AfterHostMigration()
        {
            if (!this.Runner.IsServer)
                return;
            
            foreach (MigratedTransform migratedObject in _migratedTransforms)
            {
                Transform transform = migratedObject.transform;
                transform.localPosition = migratedObject.position;
                transform.localRotation = migratedObject.rotation;
            }

            _migratedTransforms.Clear();
        }
    }
}