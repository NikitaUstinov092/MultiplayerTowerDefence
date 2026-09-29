using Fusion;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
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
            if (!HasStateAuthority || _condition == null || !_condition.IsMet())
                return;
            
            if (!_despawnTimestamp.IsRunning)
                _despawnTimestamp = TickTimer.CreateFromSeconds(Runner, _despawnDelay);
            
            else if (_despawnTimestamp.Expired(Runner))
                Runner.Despawn(Object);
        }
    }
}
