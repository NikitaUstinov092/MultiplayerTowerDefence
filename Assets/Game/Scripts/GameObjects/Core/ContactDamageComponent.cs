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
        
        public bool TryDamage(NetworkObject target)
        {
            // Урон и кулдаун - решение сервера, не полагаемся на то, что вызывающий уже проверил авторитет.
            if (!this.HasStateAuthority)
                return false;

            bool sameTick = _hitTick == this.Runner.Tick;
            if (!sameTick && !_cooldownTimestamp.ExpiredOrNotRunning(this.Runner))
                return false;
            
            if (target == null || !target.IsValid || !target.TryGetBehaviour(out HealthComponent health) ||
                !health.IsAlive || !health.CanBeDamagedBy(this.Object))
                return false;

            health.TakeDamage(_damage, this.Object);

            if (!sameTick)
            {
                _hitTick = this.Runner.Tick;
                _cooldownTimestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
            }

            return true;
        }
    }
}
