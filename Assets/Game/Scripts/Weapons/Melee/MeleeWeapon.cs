using System.Collections.Generic;
using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    public sealed class MeleeWeapon : Weapon
    {
        private static readonly List<LagCompensatedHit> s_hitsBuffer = new();

        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private float _fireRadius = 0.5f;

        [SerializeField]
        private int _damage = 1;

        [SerializeField]
        private float _cooldown = 1;

        [SerializeField]
        private LayerMask _layerMask;

        [Networked]
        private TickTimer _cooldownTimestamp { get; set; }

        public override bool CanFire()
        {
            return _cooldownTimestamp.ExpiredOrNotRunning(this.Runner);
        }

        public override void Fire()
        {
            PlayerRef inputAuthority = this.Object.InputAuthority;
            int count;
            if (inputAuthority.IsValid)
            {
                count = this.Runner.LagCompensation.OverlapSphere(
                    _firePoint.position,
                    _fireRadius,
                    inputAuthority,
                    s_hitsBuffer,
                    _layerMask,
                    HitOptions.IncludePhysX | HitOptions.SubtickAccuracy,
                    clearHits: true,
                    Ignore
                );
            }
            else
            {
                count = this.Runner.LagCompensation.OverlapSphere(
                    _firePoint.position,
                    _fireRadius,
                    this.Runner.Tick.Raw,
                    null,
                    null,
                    s_hitsBuffer,
                    _layerMask,
                    HitOptions.IncludePhysX | HitOptions.SubtickAccuracy,
                    clearHits: true,
                    Ignore
                );
            }

            for (int i = 0; i < count; i++)
            {
                LagCompensatedHit hit = s_hitsBuffer[i];
                Hitbox hitbox = hit.Hitbox;
                if (hitbox == null)
                    continue;

                NetworkObject other = hitbox.GetComponentInParent<NetworkObject>();
                if (other != null && other.TryGetBehaviour(out HealthComponent health) && health.IsAlive &&
                    health.CanBeDamagedBy(this.Object))
                {
                    health.TakeDamage(_damage, this.Object);
                    break;
                }
            }

            _cooldownTimestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
        }

        private void OnDrawGizmosSelected()
        {
            if (_firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_firePoint.position, _fireRadius);
            }
        }
    }
}