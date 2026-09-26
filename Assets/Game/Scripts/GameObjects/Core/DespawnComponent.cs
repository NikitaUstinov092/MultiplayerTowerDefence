using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class DespawnComponent : NetworkBehaviour
    {
        public interface ICondition
        {
            bool IsMet();
        }

        [SerializeField]
        private float _despawnDelay = 1f;

        [Networked]
        private TickTimer _despawnTimestamp { get; set; }

        private ICondition _condition;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        public override void FixedUpdateNetwork()
        {
            // Деспавн - решение сервера, клиентам предсказывать тут нечего.
            if (!this.HasStateAuthority || _condition == null || !_condition.IsMet())
                return;

            // Задержка нужна, чтобы клиенты успели отыграть визуал (смерть, взрыв) до исчезновения объекта.
            if (!_despawnTimestamp.IsRunning)
                _despawnTimestamp = TickTimer.CreateFromSeconds(this.Runner, _despawnDelay);
            else if (_despawnTimestamp.Expired(this.Runner))
                this.Runner.Despawn(this.Object);
        }
    }
}
