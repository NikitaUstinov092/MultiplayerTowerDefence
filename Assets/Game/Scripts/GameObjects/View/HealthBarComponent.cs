using Fusion;
using Game.Scripts.Common;
using Game.Scripts.GameObjects.Core;
using UnityEngine;

namespace Game.Scripts.GameObjects.View
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
            _healthComponent.OnHealthChanged += OnHealthChanged;
            UpdateHealth(_healthComponent.Current);
        }

        public override void Despawned(NetworkRunner runner, bool hasState) =>
            _healthComponent.OnHealthChanged -= OnHealthChanged;

        private void OnHealthChanged(int previous, int current)
        {
            Debug.Log($"Health Changed {Object.name} from {previous} to {current}");
            UpdateHealth(current);
        }

        private void UpdateHealth(int health)
        {
            // DOTWeen
            _healthBar.SetText($"{health}/{_healthComponent.Max}");
            _healthBar.SetProgress(_healthComponent.Progress);
        }
    }
}
