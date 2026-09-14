using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class GrenadeView : NetworkBehaviour
    {
        [SerializeField]
        private GrenadeBase _grenade;

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Material _blue;

        [SerializeField]
        private Material _red;

        [SerializeField]
        private GameObject[] _visuals;

        [SerializeField]
        private float _showDelay = 0.2f;

        [SerializeField]
        private ParticleSystem _explosionVfxPrefab;

        public override void Spawned()
        {
            _grenade.OnOwnerChanged += this.OnOwnerChanged;
            this.OnOwnerChanged(_grenade.Owner);
        }

        public override void Render()
        {
            int throwTick = _grenade.ThrowTick;
            bool delay = (this.Runner.Tick - throwTick) * this.Runner.DeltaTime > _showDelay; //STATE AUTHORIT
            bool isVisible = throwTick > 0 && delay;
            this.SetVisible(isVisible);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            this.SpawnExplosionVfx();
            _grenade.OnOwnerChanged -= this.OnOwnerChanged;
        }
        
        private void SpawnExplosionVfx()
        {
            ParticleSystem explosion = Instantiate(_explosionVfxPrefab, this.transform.position, this.transform.rotation);
            explosion.Play(withChildren: true);
            Destroy(explosion.gameObject, explosion.main.duration);
        }

        private void SetVisible(bool isVisible)
        {
            foreach (var visual in _visuals)
                visual.SetActive(isVisible);
        }

        private void OnOwnerChanged(PlayerRef player)
        {
            _renderer.material = player == this.Runner.LocalPlayer ? _blue : _red;
        }
    }
}