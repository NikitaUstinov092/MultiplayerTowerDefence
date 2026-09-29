using System;
using Fusion;
using Game.Scripts.Projectiles;
using Game.Scripts.Projectiles.Core;
using UnityEngine;

namespace Game.Scripts.Weapons.Projectile
{
    public sealed class ProjectileWeapon : Weapon
    {
        public event Action OnFire;

        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private float _cooldown = 1;

        [Networked]
        private TickTimer _cooldownTimestamp { get; set; }

        [Networked]
        [UnitySerializeField]
        private int _ammo { get; set; }

        [Networked]
        [UnitySerializeField]
        private ProjectileType _projectileType { get; set; }

        [SerializeField]
        private ProjectileWorld _projectileWorld;

        [Networked, UnityNonSerialized]
        private int _fireCount { get; set; }

        private int _localFireCount;

        public override bool CanFire()
        {
            return _cooldownTimestamp.ExpiredOrNotRunning(Runner) &&
                   _ammo > 0 &&
                   _projectileWorld.CanSpawn();
        }

        public override void Fire()
        {
            _projectileWorld.TrySpawn(_projectileType, _firePoint.position, _firePoint.rotation);
            _ammo--;
            _cooldownTimestamp = TickTimer.CreateFromSeconds(Runner, _cooldown);
            _fireCount++;
        }

        public override void Spawned()
        {
            _localFireCount = _fireCount;
        }

        public override void Render()
        {
            while (_localFireCount < _fireCount)
            {
                OnFire?.Invoke();
                _localFireCount++;
            }
        }
    }
}
