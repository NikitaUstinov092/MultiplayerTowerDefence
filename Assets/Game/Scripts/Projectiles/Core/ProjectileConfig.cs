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

        public abstract void OnSimulate(ref Projectile projectile,
            PlayerRef player,
            NetworkRunner runner,
            out bool finished);

        public abstract void OnGizmos(
            in Projectile projectile,
            PlayerRef player,
            NetworkRunner runner
        );
    }
}