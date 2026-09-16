using Fusion;
using UnityEngine;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace SampleGame
{
    public sealed class ProjectileWorld : NetworkBehaviour
    {
        private const int CAPACITY = 32;

        public int Length => CAPACITY;

        private ArrayReader<Projectile> s_projectileReader =
            GetArrayReader<Projectile>(typeof(ProjectileWorld), nameof(_projectiles));

        [SerializeField]
        private ProjectileCatalog _catalog;

        [Networked, Capacity(CAPACITY)]
        private NetworkArray<Projectile> _projectiles { get; }

        public bool CanSpawn() => this.FindFreeSlot(out _);

        public bool TrySpawn(ProjectileType type, Vector3 position, Quaternion rotation)
        {
            if (!this.FindFreeSlot(out int freeIndex))
                return false;

            NetworkRunner runner = this.Runner;
            Projectile projectile = new Projectile
            {
                type = type,
                startTick = runner.Tick,
                Position = position,
                Rotation = rotation,
            };

            
            ProjectileConfig config = _catalog.GetConfig(type);
            config.OnSpawned(ref projectile, this.Object.InputAuthority, runner);

            _projectiles.Set(freeIndex, projectile);
            return true;
        }

        private bool FindFreeSlot(out int index)
        {
            for (int i = 0; i < CAPACITY; i++)
            {
                Projectile projectile = _projectiles[i];
                if (!projectile.IsAlive)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public override void FixedUpdateNetwork()
        {
            NetworkRunner runner = this.Runner;
            PlayerRef player = this.Object.InputAuthority;

            for (int i = 0; i < CAPACITY; i++)
            {
                ref Projectile projectile = ref _projectiles.GetRef(i);
                if (!projectile.IsAlive)
                    continue;

                ProjectileType projectileType = projectile.type;
                ProjectileConfig config = _catalog.GetConfig(projectileType);
                config.OnSimulate(ref projectile, player, runner, out bool finished);

                if (finished)
                    projectile = default;
            }
        }

        private void OnDrawGizmos()
        {
            if (!this.StateBufferIsValid)
                return;

            PlayerRef player = this.Object.InputAuthority;
            NetworkRunner runner = this.Runner;
            for (int i = 0; i < CAPACITY; i++)
            {
                ref Projectile projectile = ref _projectiles.GetRef(i);
                if (!projectile.IsAlive)
                    continue;

                ProjectileConfig config = _catalog.GetConfig(projectile.type);
                config.OnGizmos(in projectile, player, runner);
            }
        }

        public ref readonly Projectile GetProjectileAt(int index)
        {
            return ref _projectiles.GetRef(index);
        }

        public void GetProjectileSnapshots(
            out NetworkArrayReadOnly<Projectile> previous,
            out NetworkArrayReadOnly<Projectile> current,
            out float alpha //0..1
        )
        {
            if (this.TryGetSnapshotsBuffers(
                    out NetworkBehaviourBuffer from,
                    out NetworkBehaviourBuffer to,
                    out alpha
                ))
            {
                previous = s_projectileReader.Read(from);
                current = s_projectileReader.Read(to);
            }
            else
            {
                previous = default;
                current = _projectiles;
            }
        }
    }
}