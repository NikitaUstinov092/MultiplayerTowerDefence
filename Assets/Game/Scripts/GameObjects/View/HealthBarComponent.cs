using Fusion;
using UnityEngine;

namespace SampleGame
{
    // Presenter
    public sealed class HealthBarComponent : NetworkBehaviour
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private HealthBarView _healthBar;

        public override void Spawned()
        {
            _healthComponent.OnHealthChanged += this.OnHealthChanged;
            this.UpdateHealth(_healthComponent.Current);
        }

        public override void Despawned(NetworkRunner runner, bool hasState) =>
            _healthComponent.OnHealthChanged -= this.OnHealthChanged;

        private void OnHealthChanged(int previous, int current)
        {
            Debug.Log($"Health Changed {this.Object.name} from {previous} to {current}");
            this.UpdateHealth(current);
        }

        private void UpdateHealth(int health)
        {
            // DOTWeen
            _healthBar.SetText($"{health}/{_healthComponent.Max}");
            _healthBar.SetProgress(_healthComponent.Progress);
        }
    }
}
