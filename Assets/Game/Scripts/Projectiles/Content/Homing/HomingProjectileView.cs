using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class HomingProjectileView : ProjectileView
    {
        [SerializeField]
        private Renderer[] _renderers;

        [SerializeField]
        private Material _blue;

        [SerializeField]
        private Material _red;
        
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

            this.UpdateTransform(in previous, in current, alpha);
        }

        public override void OnRender(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            this.UpdateTransform(in previous, in current, alpha);
        }

        public override void OnDespawn(in Projectile previous, PlayerRef player, NetworkRunner runner)
        {
        }
        
        private void UpdateTransform(in Projectile previous, in Projectile current, float alpha)
        {
            Vector3 position = Vector3.Lerp(previous.Position, current.Position, alpha);
            Quaternion rotation = Quaternion.Slerp(previous.Rotation, current.Rotation, alpha);
            this.transform.SetPositionAndRotation(position, rotation);
        }
    }
}