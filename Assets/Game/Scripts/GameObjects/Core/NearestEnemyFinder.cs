using Fusion;
using UnityEngine;
using static UnityEngine.QueryTriggerInteraction;

namespace SampleGame
{
    public static class NearestEnemyFinder
    {
        // Не по кол-ву объектов в матче, а по тому, сколько реально попало в сферу поиска.
        private const int MaxColliders = 32;
        private static readonly Collider[] s_colliderBuffer = new Collider[MaxColliders];

        public static bool TryFind(
            NetworkRunner runner,
            Vector3 origin,
            float radius,
            LayerMask layerMask,
            NetworkObject attacker,
            out NetworkObject target
        )
        {
            int count = runner.GetPhysicsScene()
                .OverlapSphere(origin, radius, s_colliderBuffer, layerMask, Ignore);

            float minDistance = radius * radius;
            target = null;

            for (int i = 0; i < count; i++)
            {
                NetworkObject obj = s_colliderBuffer[i].GetComponentInParent<NetworkObject>();
                if (obj == null)
                    continue;

                if (!obj.TryGetBehaviour(out HealthComponent health) || !health.IsAlive || !health.CanBeDamagedBy(attacker))
                    continue;

                // Не берём в цели того, у кого совпадает команда с атакующим
                if (obj.TryGetBehaviour(out TeamComponent targetTeam) &&
                    attacker != null && attacker.TryGetBehaviour(out TeamComponent attackerTeam) &&
                    targetTeam.Current == attackerTeam.Current)
                    continue;

                float distance = (obj.transform.position - origin).sqrMagnitude;
                if (distance > minDistance)
                    continue;

                minDistance = distance;
                target = obj;
            }

            return target != null;
        }
    }
}
