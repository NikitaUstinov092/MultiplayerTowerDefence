using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "BallisticProjectileConfig",
        menuName = "SampleGame/Projectiles/New BallisticProjectileConfig"
    )]
    public sealed class BallisticProjectileConfig : ProjectileConfig
    {
        private static readonly Collider[] s_colliderBuffer = new Collider[8];

        [SerializeField]
        private float _radius = 0.25f;

        [SerializeField]
        private int _damage;

        [SerializeField]
        private float _damageRadius = 1f;

        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private float _gravity = -9.81f;

        public override void OnSpawned(ref Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
        }

        public override void OnSimulate(ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out bool finished)
        {
            float currentTime = (runner.Tick - projectile.startTick) * runner.DeltaTime;
            float previousTime = Mathf.Max(0f, currentTime - runner.DeltaTime);

            Vector3 previousPosition = GetPosition(in projectile, previousTime);
            Vector3 currentPosition = GetPosition(in projectile, currentTime);

            PhysicsScene physicsScene = runner.GetPhysicsScene();
            finished = CheckCollision(physicsScene, previousPosition, currentPosition, out Vector3 hitPosition);

            if (finished)
                ApplyDamage(physicsScene, player, hitPosition);
        }

        public void GetRenderPositionAndRotation(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out Vector3 position,
            out Quaternion rotation
        )
        {
            float deltaTime = runner.DeltaTime;
            float spawnTime = projectile.startTick * deltaTime;

            float renderTime = runner.LocalPlayer == player || runner.IsServer
                ? runner.LocalRenderTime + deltaTime
                : runner.RemoteRenderTime + deltaTime;

            float time = Mathf.Max(0f, renderTime - spawnTime);

            position = GetPosition(in projectile, time);
            Vector3 velocity = GetVelocity(in projectile, time);

            rotation = velocity.sqrMagnitude > 0f
                ? Quaternion.LookRotation(velocity)
                : projectile.Rotation;
        }

        public override void OnGizmos(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner)
        {
            Color previousColor = Gizmos.color;

            bool hasInputAuthority = runner.LocalPlayer == player;
            Gizmos.color = hasInputAuthority
                ? Color.blue
                : Color.red;

            Tick simulationTick = hasInputAuthority || runner.IsServer ? runner.Tick : runner.LatestServerTick;
            float currentTime = (simulationTick - projectile.startTick) * runner.DeltaTime;
            currentTime = Mathf.Max(0f, currentTime);

            Vector3 position = GetPosition(in projectile, currentTime);
            Gizmos.DrawWireSphere(position, _radius);

            Gizmos.color = previousColor;
        }

        private Vector3 GetPosition(in Projectile projectile, float time)
        {
            Vector3 velocity = GetStartVelocity(in projectile);
            Vector3 acceleration = GetAcceleration();
            return projectile.Position + velocity * time + 0.5f * acceleration * time * time;
        }

        private Vector3 GetVelocity(in Projectile projectile, float time)
        {
            Vector3 velocity = GetStartVelocity(in projectile);
            Vector3 acceleration = GetAcceleration();
            return velocity + acceleration * time;
        }

        private Vector3 GetStartVelocity(in Projectile projectile)
        {
            return projectile.Direction * _speed;
        }

        private Vector3 GetAcceleration()
        {
            return Vector3.up * _gravity;
        }

        private bool CheckCollision(PhysicsScene physicsScene, Vector3 from, Vector3 to, out Vector3 hitPosition)
        {
            Vector3 delta = to - from;
            float distance = delta.magnitude;

            if (distance <= Mathf.Epsilon)
            {
                hitPosition = to;
                return physicsScene.OverlapSphere(to, _radius, s_colliderBuffer, _layerMask, Ignore) > 0;
            }

            if (physicsScene.SphereCast(from, _radius, delta / distance, out RaycastHit hit, distance, _layerMask,
                    Ignore))
            {
                hitPosition = hit.point;
                return true;
            }

            hitPosition = to;
            return false;
        }

        private void ApplyDamage(PhysicsScene physicsScene, PlayerRef player, Vector3 position)
        {
            int count = physicsScene.OverlapSphere(
                position,
                _damageRadius,
                s_colliderBuffer,
                _layerMask,
                Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider collider = s_colliderBuffer[i];

                NetworkObject networkObject = collider.GetComponentInParent<NetworkObject>();
                if (networkObject == null || networkObject.InputAuthority == player)
                    continue;

                if (networkObject.TryGetBehaviour(out HealthComponent healthComponent))
                    healthComponent.TakeDamage(_damage);
            }
        }
    }
}