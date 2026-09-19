using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerLoseComponent : NetworkBehaviour
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        public override void Spawned()
        {
            _healthComponent.OnHealthChanged += this.OnHealthChanged;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _healthComponent.OnHealthChanged -= this.OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int current)
        {
            if (previous > 0 && current <= 0)
                this.Runner.GetBehaviour<LoseNotificator>().NotifyAboutLose();
        }
    }
}
