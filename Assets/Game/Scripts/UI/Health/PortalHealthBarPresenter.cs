using Fusion;
using SampleGame;
using UnityEngine;

namespace Game
{
    // Presenter: связывает HealthComponent (логика) и SmoothHealthBar (вьюха)
    public sealed class PortalHealthBarPresenter : NetworkBehaviour
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private SmoothHealthBar _view;
        
        public override void Spawned()
        {
            _healthComponent.OnHealthChanged += this.OnHealthChanged;
            UpdateView(_healthComponent.Current, smoothFollow: false);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _healthComponent.OnHealthChanged -= this.OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int current)
        {
            this.UpdateView(current, smoothFollow: true);
        }

        private void UpdateView(int current, bool smoothFollow)
        {
            _view.SetCaption($"{current}/{_healthComponent.Max}");
            _view.Set(_healthComponent.Progress, smoothFollow);
        }
    }
}
