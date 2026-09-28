using Fusion;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.GameObjects.Core.Move;
using Game.Scripts.GameSystems.Player;
using UnityEngine;

namespace Game.Scripts.GameObjects
{
    public sealed class Player : NetworkBehaviour,
        MoveComponent.ICondition,
        WeaponComponent.ICondition,
        LoseComponent.ICondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        [SerializeField]
        private LoseComponent _loseComponent;

        public override void Spawned()
        {
            _moveComponent.SetCondition(this);
            _weaponComponent.SetCondition(this);
            _loseComponent.SetCondition(this);

            if (_teamComponent != null)
                _healthComponent.SetDamageCondition(_teamComponent);
        }

        bool MoveComponent.ICondition.IsMet()
        {
            return _healthComponent.IsAlive;
        }

        bool WeaponComponent.ICondition.IsMet()
        {
            return !_moveComponent.IsMoving && _healthComponent.IsAlive;
        }

        bool LoseComponent.ICondition.IsMet()
        {
            return _healthComponent.IsDead;
        }
    }
}