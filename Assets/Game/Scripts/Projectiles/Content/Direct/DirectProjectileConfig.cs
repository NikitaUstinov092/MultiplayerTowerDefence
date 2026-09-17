using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "ProjectileConfig",
        menuName = "SampleGame/Projectiles/New DirectProjectileConfig"
    )]
    public sealed class DirectProjectileConfig : ProjectileConfig
    {
        [SerializeField]
        private int _damage = 1;

        [SerializeField]
        private float _speed = 5;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private float _lifetime = 5;

        public override void OnSpawned(ref Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
        }

        public override void OnSimulate(ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out bool finished)
        {
            finished = false;

            float deltaTime = runner.DeltaTime;
            float currentTime = (runner.Tick - projectile.startTick) * deltaTime;
            if (currentTime > _lifetime)
            {
                finished = true;
                return;
            }

            float previousTime = Mathf.Max(0, currentTime - deltaTime);
            Vector3 direction = projectile.Direction;
            Vector3 position = projectile.Position + direction * previousTime * _speed;

            bool wasHit = runner
                .GetPhysicsScene()
                .Raycast(position, direction, out RaycastHit hit, _speed * deltaTime, _layerMask, Ignore);

            if (wasHit && !this.IsFriendly(hit.collider, player))
            {
                this.DealDamage(hit.collider, player);
                finished = true;
            }
        }

        private bool IsFriendly(Collider collider, PlayerRef player)
        {
            NetworkObject target = collider.GetComponentInParent<NetworkObject>();
            return target != null &&
                   target.TryGetBehaviour(out HealthComponent healthComponent) &&
                   !healthComponent.CanBeDamagedBy(player);
        }

        private void DealDamage(Collider collider, PlayerRef player)
        {
            NetworkObject target = collider.GetComponentInParent<NetworkObject>();
            if (target == null || !target.TryGetBehaviour(out HealthComponent healthComponent))
                return;

            healthComponent.TakeDamage(_damage, player);
        }

        public override void OnGizmos(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        )
        {
            bool hasInputAuthority = runner.LocalPlayer == player;

            Color prevColor = Gizmos.color;
            Color color = hasInputAuthority ? Color.blue : Color.red;

            Gizmos.color = color;

            Tick simulationTick = hasInputAuthority || runner.IsServer ? runner.Tick : runner.LatestServerTick;
            float t = (simulationTick - projectile.startTick) * runner.DeltaTime;

            Vector3 position = projectile.Position + projectile.Direction * t * _speed;

            const float radius = 0.125f;
            Gizmos.DrawSphere(position, radius);
            Gizmos.color = prevColor;
        }

        public Vector3 GetRenderPosition(in Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
            float deltaTime = runner.DeltaTime;
            float spawnTime = projectile.startTick * deltaTime;

            float renderTime = runner.LocalPlayer == player || runner.IsServer
                ? runner.LocalRenderTime + deltaTime
                : runner.RemoteRenderTime + deltaTime;
            float t = renderTime - spawnTime;
            return projectile.Position + projectile.Direction * t * _speed;
        }
    }
}