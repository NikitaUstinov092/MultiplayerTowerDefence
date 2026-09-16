
using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "RicochetProjectileConfig",
        menuName = "SampleGame/Projectiles/New RicochetProjectileConfig"
    )]
    public sealed class RicochetProjectileConfig : ProjectileConfig
    {
        private const float SURFACE_OFFSET = 0.01f;

        [SerializeField]
        private uint _ricochetCount = 3;

        [SerializeField]
        private float _moveSpeed = 5f;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private int _damage;

        public override void OnSpawned(ref Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
            projectile.ReinterpretData<RicochetProjectileData>().ricochetCount = _ricochetCount;
        }

        public override void OnSimulate(
            ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out bool finished
        )
        {
            finished = false;

            float distance = _moveSpeed * runner.DeltaTime;
            PhysicsScene scene = runner.GetPhysicsScene();

            while (distance > 0f)
            {
                Vector3 direction = projectile.Direction;
                if (!scene.Raycast(projectile.Position, direction, out RaycastHit hit, distance, _layerMask, Ignore))
                {
                    projectile.Position += direction * distance;
                    return;
                }

                distance -= hit.distance;

                ref uint ricochetCount = ref projectile.ReinterpretData<RicochetProjectileData>().ricochetCount;
                if (this.DealDamage(hit.collider, player) || ricochetCount <= 0)
                {
                    projectile.Position = hit.point;
                    finished = true;
                    return;
                }

                ricochetCount--;

                direction = Vector3.Reflect(direction, hit.normal).normalized;
                projectile.Position = hit.point + direction * SURFACE_OFFSET;
                projectile.Rotation = Quaternion.LookRotation(direction);
            }
        }

        private bool DealDamage(Collider collider, PlayerRef player)
        {
            NetworkObject target = collider.GetComponentInParent<NetworkObject>();
            if (target == null || target.InputAuthority == player ||
                !target.TryGetBehaviour(out HealthComponent healthComponent))
                return false;

            healthComponent.TakeDamage(_damage);
            return true;
        }

        public override void OnGizmos(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = player == runner.LocalPlayer ? Color.blue : Color.red;
            Gizmos.DrawSphere(projectile.Position, radius: 0.125f);
            Gizmos.color = prevColor;
        }
    }
}
