using Fusion;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class ContactDamageComponent : NetworkBehaviour
    {
        [SerializeField]
        private int _damage = 1;

        [SerializeField]
        private float _cooldown = 1f;

        [Networked]
        private TickTimer _cooldownTimestamp { get; set; }

        [Networked]
        private Tick _hitTick { get; set; }

        // Вызывается до Spawned (onBeforeSpawned) на сервере; [Networked] не нужен,
        // т.к. TryDamage выполняется только на state authority.
        public void Init(int damage, float cooldown)
        {
            _damage = damage;
            _cooldown = cooldown;
        }

        public bool TryDamage(NetworkObject target)
        {
            if (!HasStateAuthority)
                return false;

            bool sameTick = _hitTick == Runner.Tick;
            if (!sameTick && !_cooldownTimestamp.ExpiredOrNotRunning(Runner))
                return false;
            
            if (target == null || !target.IsValid || !target.TryGetBehaviour(out HealthComponent health) ||
                !health.IsAlive || !health.CanBeDamagedBy(Object))
                return false;

            health.TakeDamage(_damage, Object);

            if (!sameTick)
            {
                _hitTick = Runner.Tick;
                _cooldownTimestamp = TickTimer.CreateFromSeconds(Runner, _cooldown);
            }

            return true;
        }
    }
}
