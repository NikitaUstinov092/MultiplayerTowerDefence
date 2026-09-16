using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class ProjectileWorldView : NetworkBehaviour
    {
        [SerializeField]
        private ProjectileWorld _world;

        [SerializeField]
        private Transform _container;

        private ProjectileViewPool _projectileViewPool;

        private ProjectileView[] _projectileViews;

        public override void Spawned()
        {
            _projectileViews = new ProjectileView[_world.Length];
            _projectileViewPool = this.Runner.GetBehaviour<ProjectileViewPool>();
        }

        public override void Render()
        {
            PlayerRef player = _world.Object.InputAuthority;
            NetworkRunner runner = this.Runner;

            _world.GetProjectileSnapshots(
                out NetworkArrayReadOnly<Projectile> previousProjectiles,
                out NetworkArrayReadOnly<Projectile> currentProjectiles,
                out float alpha
            );

            for (int i = 0; i < _world.Length; i++)
            {
                Projectile previousProjectile = previousProjectiles[i];
                Projectile currentProjectile = currentProjectiles[i];

                if (!previousProjectile.IsAlive)
                    previousProjectile = currentProjectile;


                bool hasProjectile = currentProjectile.IsAlive;

                ref ProjectileView view = ref _projectileViews[i];
                bool hasView = view != null;

                if (hasProjectile && !hasView)
                {
                    // Spawn
                    view = _projectileViewPool.Rent(currentProjectile.type, _container);
                    view.OnSpawn(in previousProjectile, in currentProjectile, alpha, player, runner);
                }
                else if (hasView && !hasProjectile)
                {
                    // Despawn
                    view.OnDespawn(in previousProjectile, player, runner);
                    _projectileViewPool.Return(view);
                    view = null;
                }
                else if (hasProjectile)
                {
                    // Update
                    view.OnRender(in previousProjectile, in currentProjectile, alpha, player, runner);
                }
            }
        }
    }
}