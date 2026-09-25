using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class DespawnOnDeathComponent : NetworkBehaviour
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private float _despawnDelay = 1f;

        [Networked]
        private TickTimer _despawnTimestamp { get; set; }

        public override void FixedUpdateNetwork()
        {
            // Деспавн трупа - решение сервера, клиентам предсказывать тут нечего.
            if (!this.HasStateAuthority || !_healthComponent.IsDead)
                return;

            if (!_despawnTimestamp.IsRunning)
                _despawnTimestamp = TickTimer.CreateFromSeconds(this.Runner, _despawnDelay);
            else if (_despawnTimestamp.Expired(this.Runner))
                this.Runner.Despawn(this.Object);
        }
    }
}
