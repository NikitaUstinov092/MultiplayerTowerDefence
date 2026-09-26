using Fusion;
using UnityEngine;

namespace SampleGame
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

        // Вызывать только на StateAuthority.
        public bool TryDamage(NetworkObject target)
        {
            // За один тик враг может задеть несколько целей (портал и игрока) - бьём всех,
            // кулдаун запускается один раз на весь тик.
            bool sameTick = _hitTick == this.Runner.Tick;
            if (!sameTick && !_cooldownTimestamp.ExpiredOrNotRunning(this.Runner))
                return false;

            // IsValid: коллайдеры деспавненного в этом тике объекта ещё остаются в PhysX-сцене.
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
