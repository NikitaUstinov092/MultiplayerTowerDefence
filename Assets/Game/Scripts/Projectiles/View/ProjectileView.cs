using Fusion;
using Game.Scripts.Projectiles.Core;
using UnityEngine;

namespace Game.Scripts.Projectiles.View
{
    public abstract class ProjectileView : MonoBehaviour
    {
        public abstract void OnSpawn(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        );

        public abstract void OnRender(
            in Projectile previous,
            in Projectile current,
            float alpha,
            PlayerRef player,
            NetworkRunner runner
        );

        public abstract void OnDespawn(
            in Projectile previous,
            PlayerRef player,
            NetworkRunner runner
        );
    }
}