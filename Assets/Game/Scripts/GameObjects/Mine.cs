using Fusion;
using SampleGame;
using UnityEngine;

namespace Game.Scripts.GameObjects
{
    public sealed class Mine : NetworkBehaviour,
        CapsuleCollisionComponent.IHandler,
        ExplosionComponent.IHandler,
        DespawnComponent.ICondition
    {
        [SerializeField]
        private TeamComponent _teamComponent;

        [SerializeField]
        private CapsuleCollisionComponent _collisionComponent;

        [SerializeField]
        private ExplosionComponent _explosionComponent;

        [SerializeField]
        private ContactDamageComponent _contactDamage;

        [SerializeField]
        private DespawnComponent _despawnComponent;

        public override void Spawned()
        {
            _collisionComponent.SetHandler(this);
            _explosionComponent.SetHandler(this);
            _despawnComponent.SetCondition(this);
        }

        bool DespawnComponent.ICondition.IsMet() => _explosionComponent.IsExploded;

        void CapsuleCollisionComponent.IHandler.OnCollision(NetworkObject other)
        {
            if (_explosionComponent.IsExploded)
                return;

            // Детонация только от объекта чужой команды; объекты без команды (портал, снаряды) мину не трогают.
            if (!other.TryGetBehaviour(out TeamComponent otherTeam) || otherTeam.Current == _teamComponent.Current)
                return;

            _explosionComponent.Explode();
        }

        void ExplosionComponent.IHandler.OnHit(NetworkObject target)
        {
            // Проверку команды цели делает HealthComponent.CanBeDamagedBy внутри TryDamage.
            _contactDamage.TryDamage(target);
        }
    }
}
