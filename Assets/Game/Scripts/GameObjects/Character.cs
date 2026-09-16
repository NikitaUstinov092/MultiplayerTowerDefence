using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class Character : NetworkBehaviour,
        MoveComponent.ICondition,
        WeaponComponent.ICondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private WeaponComponent _weaponComponent;

        public override void Spawned()
        {
            _moveComponent.SetCondition(this);
            _weaponComponent.SetCondition(this);
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
    }
}