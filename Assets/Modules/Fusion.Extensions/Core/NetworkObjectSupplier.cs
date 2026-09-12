using System;
using UnityEngine;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Fusion
{
    public class NetworkObjectSupplier : NetworkBehaviour
    {
        private const int CAPACITY = 8;

        [SerializeField]
        private NetworkObject _prefab;

        [Networked, Capacity(CAPACITY)]
        private NetworkArray<NetworkObject> _queue { get; }

        [SerializeField]
        private Transform _container;

        [Networked]
        private int _headIndex { get; set; }

        private readonly NetworkObject[] _localQueue = new NetworkObject[CAPACITY];

        [SerializeField]
        private Vector3 _outPosition = new(0f, -1000f, 0f);

        public bool CanSupply => _queue[_headIndex] != null;

        public bool Supply(PlayerRef inputAuthority, out NetworkObject instance)
        {
            instance = _queue[_headIndex];
            if (instance == null)
                return false;

            // Activate instance
            instance.AssignInputAuthority(inputAuthority);
            this.OnSupplied(instance, withAuthority: true);

            // Spawn new instance
            if (this.HasStateAuthority)
                _queue.Set(_headIndex, this.SpawnInstance());

            // Move head to next
            _headIndex = (_headIndex + 1) % _queue.Length;
            return true;
        }

        protected virtual void OnSupplied(NetworkObject instance, bool withAuthority)
        {
            if (withAuthority)
                instance.SetIsSimulated(true);

            instance.gameObject.SetActive(true);
        }

        protected virtual void OnSpawned(NetworkObject instance, bool withAuthority)
        {
            if (withAuthority) 
                instance.SetIsSimulated(false);
            
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container);
        }

        private NetworkObject SpawnInstance()
        {
            NetworkObject instance = this.Runner.Spawn(_prefab, _outPosition, Quaternion.identity);
            this.OnSpawned(instance, true);
            return instance;
        }

        public override void Spawned()
        {
            if (this.HasStateAuthority)
                for (int i = 0; i < _queue.Length; i++)
                    _queue.Set(i, this.SpawnInstance());
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (this.HasStateAuthority && hasState)
            {
                foreach (NetworkObject instance in _queue)
                    if (instance != null)
                        this.Runner.Despawn(instance);

                _queue.Clear();
            }

            Array.Clear(_localQueue, 0, _localQueue.Length);
        }

        public override void Render()
        {
            if (this.HasStateAuthority)
                return;

            for (int i = 0; i < _queue.Length; i++)
            {
                NetworkObject localInstance = _localQueue[i];
                NetworkObject remoteInstance = _queue[i];

                if (localInstance == remoteInstance)
                    continue;

                // Show a previous object
                if (localInstance != null && localInstance.IsValid)
                    this.OnSupplied(localInstance, withAuthority: false);

                // Hide a new object
                if (remoteInstance != null && remoteInstance.IsValid)
                    this.OnSpawned(remoteInstance, withAuthority: false);

                _localQueue[i] = remoteInstance;
            }
        }
    }
}