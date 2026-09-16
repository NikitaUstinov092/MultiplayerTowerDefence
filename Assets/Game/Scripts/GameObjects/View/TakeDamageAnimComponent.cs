using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class TakeDamageAnimComponent : NetworkBehaviour
    {
        private static readonly int TakeDamage = Animator.StringToHash(nameof(TakeDamage));

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private HealthComponent _healthComponent;

        public override void Spawned()
        {
            _healthComponent.OnDamageTaken += this.OnDamageTaken;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _healthComponent.OnDamageTaken -= this.OnDamageTaken;
        }

        private void OnDamageTaken()
        {
            if (_healthComponent.IsAlive)
            {
                //TODO в Animator Controller нет параметра TakeDamage, добавить и раскомментировать
                // _animator.SetTrigger(TakeDamage);
            }
        }
    }
}
