using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class DirectProjectileView : ProjectileView
    {
        [SerializeField]
        private DirectProjectileConfig _config;
        
        public override void OnSpawn(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            this.UpdatePosition(in current, player, runner);
        }

        public override void OnRender(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            this.UpdatePosition(in current, player, runner);
        }

        public override void OnDespawn(in Projectile previous, PlayerRef player, NetworkRunner runner)
        {
        }

        private void UpdatePosition(in Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
            this.transform.position = _config.GetRenderPosition(in projectile, player, runner);
            this.transform.rotation = Quaternion.LookRotation(projectile.Direction);
        }
    }
}