using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace SampleGame
{
    public static class NearestEnemyFinder
    {
        public static bool TryFind(
            NetworkRunner runner,
            Vector3 origin,
            float radius,
            NetworkObject attacker,
            out NetworkObject target
        )
        {
            List<NetworkObject> buffer = ListPool<NetworkObject>.Get();
            runner.GetAllNetworkObjects(buffer);

            float minDistance = radius * radius;
            target = null;

            for (int i = 0, count = buffer.Count; i < count; i++)
            {
                NetworkObject obj = buffer[i];
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

            ListPool<NetworkObject>.Release(buffer);

            return target != null;
        }
    }
}
