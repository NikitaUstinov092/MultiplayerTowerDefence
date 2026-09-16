using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class BallisticProjectileView : ProjectileView
    {
        [SerializeField]
        private BallisticProjectileConfig _config;

        [SerializeField]
        private Renderer[] _renderers;

        [SerializeField]
        private Material _blue;

        [SerializeField]
        private Material _red;

        [SerializeField]
        private ParticleSystem _explosionVfxPrefab;
        
        public override void OnSpawn(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            Material material = runner.LocalPlayer == player ? _blue : _red;
            foreach (Renderer renderer in _renderers)
                renderer.material = material;

            this.UpdateTransform(in current, player, runner);
        }

        public override void OnRender(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            this.UpdateTransform(in current, player, runner);
        }

        public override void OnDespawn(in Projectile previous, PlayerRef player, NetworkRunner runner)
        {
            _config.GetRenderPositionAndRotation(
                in previous,
                player,
                runner,
                out Vector3 position,
                out Quaternion rotation
            );

            Instantiate(
                _explosionVfxPrefab,
                position,
                rotation
            );
        }

        private void UpdateTransform(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            _config.GetRenderPositionAndRotation(
                in projectile,
                player,
                runner,
                out Vector3 position,
                out Quaternion rotation
            );

            this.transform.SetPositionAndRotation(position, rotation);
        }
    }
}