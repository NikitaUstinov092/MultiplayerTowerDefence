using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class HealthAnimComponent : NetworkBehaviour
    {
        private static readonly int Health = Animator.StringToHash(nameof(Health));

        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private Animator _animator;

        public override void Spawned()
        {
            _healthComponent.OnHealthChanged += this.OnHealthChanged;
            //TODO в Animator Controller нет параметра Health, добавить и раскомментировать
            // _animator.SetInteger(Health, _healthComponent.Current);
        }

        public override void Despawned(NetworkRunner runner, bool hasState) =>
            _healthComponent.OnHealthChanged -= this.OnHealthChanged;

        private void OnHealthChanged(int previous, int health)
        {
            //TODO в Animator Controller нет параметра Health, добавить и раскомментировать
            // _animator.SetInteger(Health, health);
        }
    }
}
