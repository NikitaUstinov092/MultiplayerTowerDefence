using Fusion;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.GameObjects.Core.Move;
using UnityEngine;

namespace Game.Scripts.GameObjects
{
    public sealed class Archer : NetworkBehaviour,
        MoveComponent.ICondition,
        WeaponComponent.ICondition,
        LifetimeComponent.IHandler,
        DespawnComponent.ICondition
    {
        [SerializeField]
        private HealthComponent _healthComponent;

        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private TeamComponent _teamComponent;

        [SerializeField]
        private LifetimeComponent _lifetimeComponent;

        [SerializeField]
        private DespawnComponent _despawnComponent;

        public override void Spawned()
        {
            _weaponComponent.SetCondition(this);
            _lifetimeComponent.SetHandler(this);
            _despawnComponent.SetCondition(this);

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

        void LifetimeComponent.IHandler.OnExpired()
        {
            _healthComponent.Kill();
        }

        bool DespawnComponent.ICondition.IsMet() => _healthComponent.IsDead;
    }
}
