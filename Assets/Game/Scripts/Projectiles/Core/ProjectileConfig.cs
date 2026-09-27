using Fusion;
using UnityEngine;

namespace SampleGame
{
    public abstract class ProjectileConfig : ScriptableObject
    {
        public abstract void OnSpawned(
            ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        );

        // owner — стрелявший объект (носитель ProjectileWorld), по нему проверяется команда.
        // PlayerRef для этого не годится: у серверных юнитов (лучник) нет InputAuthority.
        public abstract void OnSimulate(ref Projectile projectile,
            PlayerRef player,
            NetworkObject owner,
            NetworkRunner runner,
            out bool finished);

        public abstract void OnGizmos(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        );
    }
}