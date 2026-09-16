using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class RicochetProjectileView : ProjectileView
    {
        [SerializeField]
        private Renderer[] _renderers;

        [SerializeField]
        private Material _blue;

        [SerializeField]
        private Material _red;

        [SerializeField]
        private ParticleSystem _ricochetEffect;

        private uint _ricochetCount;

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

            _ricochetCount = current.ReinterpretData<RicochetProjectileData>().ricochetCount;
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

            uint ricochetCount = current.ReinterpretData<RicochetProjectileData>().ricochetCount;
            if (_ricochetCount > ricochetCount)
            {
                this.PlayRicochetEffect(in current);
                _ricochetCount = ricochetCount;
            }
        }

        public override void OnDespawn(
            in Projectile previous,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            this.PlayRicochetEffect(in previous);
        }

        private void UpdateTransform(in Projectile previous, in Projectile current, float alpha)
        {
            Vector3 position = Vector3.Lerp(previous.Position, current.Position, alpha);
            Quaternion rotation = Quaternion.Slerp(previous.Rotation, current.Rotation, alpha);
            this.transform.SetPositionAndRotation(position, rotation);
        }

        private void PlayRicochetEffect(in Projectile projectile)
        {
            ParticleSystem effect = Instantiate(_ricochetEffect, projectile.Position, projectile.Rotation);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
}