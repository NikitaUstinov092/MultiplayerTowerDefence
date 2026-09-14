using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class GrenadeWeapon : Weapon
    {
        private static readonly Vector3 OUT_SCENE_POSITION = new Vector3(1000, 1000, 1000);
        
        [SerializeField]
        private GrenadeBase _grenadePrefab;

        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private Vector3 _rotationOffset;

        [SerializeField]
        private Vector3 _force = new(0, 5, 10);

        [SerializeField]
        private float _cooldown = 1.5f;

        [Networked]
        private GrenadeBase _networkGrenade { get; set; }

        private GrenadeBase _localGrenade;

        [Networked]
        private TickTimer _cooldownTimestamp { get; set; }

        public override void Spawned()
        {
            _cooldownTimestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
        }

        public override void FixedUpdateNetwork()
        {
            if (this.HasStateAuthority && _networkGrenade == null)
                this.PreSpawnGrenade();
        }

        private void PreSpawnGrenade()
        {
            _networkGrenade = this.Runner.Spawn(_grenadePrefab, OUT_SCENE_POSITION, Quaternion.identity);
            _networkGrenade.Object.SetIsSimulated(false);
            _networkGrenade.gameObject.SetActive(false);
            _networkGrenade.Owner = this.Object.InputAuthority;
        }

        public override void Render()
        {
            if (this.HasStateAuthority)
                return;

            if (_localGrenade != _networkGrenade)
            {
                // Activate previous grenade
                if (_localGrenade != null)
                    _localGrenade.gameObject.SetActive(true);

                // Hide pre-spawned grenade on a client
                if (_networkGrenade != null)
                    _networkGrenade.gameObject.SetActive(false);

                _localGrenade = _networkGrenade;
            }
        }

        public override bool CanFire()
        {
            return _networkGrenade != null && _cooldownTimestamp.ExpiredOrNotRunning(this.Runner);
        }

        public override void Fire()
        {
            if (_networkGrenade == null || _cooldownTimestamp.IsRunning(this.Runner))
                return;

            this.LaunchGrenade();
            _cooldownTimestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
        }

        private void LaunchGrenade()
        {
            // TODO: Use for forecasts
            // if (this.Runner.IsForward)
            // {
            //     this.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            //     _networkGrenade.Teleport(position, rotation);
            //     _networkGrenade.Object.SetIsSimulated(true);
            //     _networkGrenade.Object.AssignInputAuthority(this.Object.InputAuthority);
            //     _networkGrenade.gameObject.SetActive(true);
            // }
            
            this.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            _networkGrenade.Teleport(position, rotation);
            _networkGrenade.Object.SetIsSimulated(true);
            _networkGrenade.Object.AssignInputAuthority(this.Object.InputAuthority);
            _networkGrenade.gameObject.SetActive(true);

            _networkGrenade.Throw(_firePoint.TransformDirection(_force));
            _networkGrenade = null;
        }

        private void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = _firePoint.position;
            rotation = _firePoint.rotation * Quaternion.Euler(_rotationOffset);
        }
    }
}