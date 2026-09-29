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
        
        public void Init(int damage, float cooldown)
        {
            _damage = damage;
            _cooldown = cooldown;
        }

        public bool TryDamage(NetworkObject target)
        {
            if (!HasStateAuthority)
                return false;

            if (target == null || !target.IsValid || !target.TryGetBehaviour(out HealthComponent health) ||
                !health.IsAlive || !health.CanBeDamagedBy(Object))
                return false;

            health.TakeDamage(_damage, Object);
            return true;
        }

        public bool TryDamageWithCooldown(NetworkObject target)
        {
            if (!HasStateAuthority)
                return false;

            // Кулдаун общий на компонент: в тике первого удара пропускаем его,
            // чтобы урон получили все цели, касающиеся в этот тик.
            
            bool sameTick = _hitTick == Runner.Tick;
            if (!sameTick && !_cooldownTimestamp.ExpiredOrNotRunning(Runner))
                return false;

            if (!TryDamage(target))
                return false;

            if (!sameTick)
            {
                _hitTick = Runner.Tick;
                _cooldownTimestamp = TickTimer.CreateFromSeconds(Runner, _cooldown);
            }

            return true;
        }
    }
}
