using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    public sealed class MeleeWeapon : Weapon
    {
        private static readonly Collider[] s_colliders = new Collider[32];

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
            // Удар инициирует сервер, цель ищется по PhysX-коллайдерам на текущем тике
            // (NearestEnemyFinder / CapsuleCollisionComponent) - бьём по той же геометрии и в тот же момент.
            int count = this.Runner.GetPhysicsScene()
                .OverlapSphere(_firePoint.position, _fireRadius, s_colliders, _layerMask, Ignore);

            for (int i = 0; i < count; i++)
            {
                NetworkObject other = s_colliders[i].GetComponentInParent<NetworkObject>();
                // IsValid: коллайдеры деспавненного в этом тике объекта ещё в PhysX-сцене.
                if (other != null && other.IsValid && other.TryGetBehaviour(out HealthComponent health) && health.IsAlive &&
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
