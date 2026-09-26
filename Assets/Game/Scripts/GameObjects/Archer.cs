using Fusion;
using SampleGame;
using UnityEngine;

namespace Game.Scripts.GameObjects
{
    public sealed class Archer : NetworkBehaviour,
        MoveComponent.ICondition,
        WeaponComponent.ICondition,
        WeaponComponent.IIdleCondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        public override void Spawned()
        {
            _weaponComponent.SetCondition(this);
            _weaponComponent.SetIdleCondition(this);

            if (_teamComponent != null)
                _healthComponent.SetDamageCondition(_teamComponent);
        }

        bool MoveComponent.ICondition.IsMet()
        {
            return _healthComponent.IsAlive;
        }

        bool WeaponComponent.ICondition.IsMet()
        {
            return _healthComponent.IsAlive;
        }

        bool WeaponComponent.IIdleCondition.IsMet()
        {
            return _healthComponent.IsAlive;
        }
    }
}
