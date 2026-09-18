using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class Character : NetworkBehaviour,
        MoveComponent.ICondition,
        WeaponComponent.ICondition,
        WeaponComponent.IIdleCondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        public override void Spawned()
        {
            _moveComponent.SetCondition(this);
            _weaponComponent.SetCondition(this);
            _weaponComponent.SetIdleCondition(this);

            if (_teamComponent != null)
                _healthComponent.SetDamageCondition(_teamComponent);
        }

        bool MoveComponent.ICondition.IsMet()
        {
            if(_healthComponent == null)
                return true;
            return _healthComponent.IsAlive;
        }

        bool WeaponComponent.ICondition.IsMet()
        {
            if(_healthComponent == null)
                return true;
            return _healthComponent.IsAlive;
        }

        bool WeaponComponent.IIdleCondition.IsMet()
        {
            return !_moveComponent.IsMoving;
        }
    }
}