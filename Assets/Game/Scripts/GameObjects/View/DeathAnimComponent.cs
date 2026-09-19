using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class DeathAnimComponent : NetworkBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash(nameof(IsDead));

        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private Animator _animator;

        public override void Spawned()
        {
            _healthComponent.OnHealthChanged += this.OnHealthChanged;
            this.OnHealthChanged(_healthComponent.Current, _healthComponent.Current);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _healthComponent.OnHealthChanged -= this.OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int current)
        {
            _animator.SetBool(IsDead, _healthComponent.IsDead);
        }
    }
}
