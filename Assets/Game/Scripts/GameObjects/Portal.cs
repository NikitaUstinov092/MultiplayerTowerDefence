using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class Portal : NetworkBehaviour
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        public override void Spawned()
        {
            // Без условия HealthComponent пропускает урон от любого атакующего, включая мины и снаряды своей команды.
            if (_teamComponent != null)
                _healthComponent.SetDamageCondition(_teamComponent);
        }
    }
}
