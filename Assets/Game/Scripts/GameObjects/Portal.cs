using Fusion;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.GameSystems.Player;
using UnityEngine;

namespace Game.Scripts.GameObjects
{
    public sealed class Portal : NetworkBehaviour,
        LoseComponent.ICondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        [SerializeField]
        private LoseComponent _loseComponent;

        public override void Spawned()
        {
            // Без условия HealthComponent пропускает урон от любого атакующего, включая мины и снаряды своей команды.
            if (_teamComponent != null)
                _healthComponent.SetDamageCondition(_teamComponent);

            if (_loseComponent != null)
                _loseComponent.SetCondition(this);
        }

        bool LoseComponent.ICondition.IsMet()
        {
            return _healthComponent.IsDead;
        }
    }
}
