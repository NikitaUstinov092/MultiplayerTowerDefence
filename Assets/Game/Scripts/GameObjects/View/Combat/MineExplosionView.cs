using Fusion;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.VFX;
using UnityEngine;

namespace Game.Scripts.GameObjects.View.Combat
{
    public sealed class MineExplosionView : NetworkBehaviour
    {
        [SerializeField]
        private ExplosionComponent _explosion;

        [SerializeField]
        private GameObject _visual;

        [SerializeField]
        private ParticleSpawner _explosionSpawner;

        public override void Spawned()
        {
            _explosion.OnExploded += this.OnExploded;
            _visual.SetActive(!_explosion.IsExploded);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _explosion.OnExploded -= this.OnExploded;
        }

        public override void Render()
        {
            _visual.SetActive(!_explosion.IsExploded);
        }

        private void OnExploded() =>
            _explosionSpawner.Play();
    }
}
