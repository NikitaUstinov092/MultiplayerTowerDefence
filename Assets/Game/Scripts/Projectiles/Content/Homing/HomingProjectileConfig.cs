using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "HomingProjectileConfig",
        menuName = "SampleGame/Projectiles/New HomingProjectileConfig"
    )]
    public sealed class HomingProjectileConfig : ProjectileConfig
    {
        [SerializeField]
        private float _detectionRadius = 20;

        [SerializeField]
        private float _moveSpeed = 10;

        [SerializeField]
        private int _damage = 5;

        [SerializeField]
        private Vector3 _targetOffset = new(0, 1, 0);

        [SerializeField]
        private float _homingDelay = 2;

        public override void OnSpawned(
            ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        )
        {
        }

        private bool FindClosestTarget(
            Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out NetworkObject target
        )
        {
            List<NetworkObject> buffer = ListPool<NetworkObject>.Get();
            runner.GetAllNetworkObjects(buffer);

            float minValue = float.MaxValue;
            target = null;

            float detectionRadius = _detectionRadius * _detectionRadius;
            for (int i = 0, count = buffer.Count; i < count; i++)
            {
                NetworkObject obj = buffer[i];
                if (obj.InputAuthority == player || !obj.TryGetBehaviour(out HealthComponent health) || !health.IsAlive)
                    continue;

                Vector3 destination = obj.transform.position;
                Vector3 delta = destination - projectile.Position;

                float distance = delta.sqrMagnitude;
                if (distance > detectionRadius)
                    continue;

                if (distance < minValue)
                {
                    target = obj;
                    minValue = distance;
                }
            }

            ListPool<NetworkObject>.Release(buffer);

            return target != null;
        }

        public override void OnSimulate(
            ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out bool finished
        )
        {
            finished = false;

            float deltaTime = runner.DeltaTime;
            float moveStep = _moveSpeed * deltaTime;

            float currentTime = (runner.Tick - projectile.startTick) * deltaTime;
            if (currentTime >= _homingDelay)
            {
                NetworkId targetId = projectile.ReinterpretData<HomingProjectileData>().target;

                if ((targetId.IsValid && runner.TryFindObject(targetId, out NetworkObject target) ||
                     this.FindClosestTarget(projectile, player, runner, out target)) &&
                    target.TryGetBehaviour(out HealthComponent health) && health.IsAlive)
                {
                    Vector3 currentPosition = projectile.Position;
                    Vector3 targetPosition = target.transform.position + _targetOffset;
                    Vector3 delta = targetPosition - currentPosition;

                    if (delta.sqrMagnitude < moveStep * moveStep)
                    {
                        health.TakeDamage(_damage);

                        finished = true;
                        return;
                    }

                    projectile.Rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
                    projectile.ReinterpretData<HomingProjectileData>().target = target;
                }
            }

            projectile.Position += projectile.Rotation * Vector3.forward * moveStep;
        }

        public override void OnGizmos(in Projectile projectile, PlayerRef player, NetworkRunner runner)
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = player == runner.LocalPlayer ? Color.blue : Color.red;
            Gizmos.DrawSphere(projectile.Position, radius: 0.125f);
            Gizmos.color = prevColor;
        }
    }
}