using System;
using Fusion;
using Game.Scripts.Weapons;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class WeaponComponent : NetworkBehaviour
    {
        private const float MinRotationDirectionSqrMagnitude = 0.0001f;
        
        public interface ICondition
        {
            bool IsMet();
        }

        public event Action OnFireStarted;

        [Networked, UnitySerializeField]
        private Weapon Current { get; set; }

        [Networked]
        private NetworkObject Target { get; set; }

        [SerializeField]
        private float _detectionRadius = 15f;

        [SerializeField]
        private LayerMask _detectionLayerMask;
        
        [SerializeField]
        private float _rangeFireDelay = 0.2f;
        
        [Networked]
        private TickTimer _delayTimestamp { get; set; }

        [Networked]
        private ushort _fireStartedEvents { get; set; }

        private ushort _localFireStartedEvents;

        private ICondition _condition;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        public override void Spawned()
        {
            _localFireStartedEvents = _fireStartedEvents;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && !_delayTimestamp.IsRunning)
            {
                Target = CanFire() &&
                              NearestEnemyFinder.TryFind(Runner, transform.position, _detectionRadius, _detectionLayerMask, Object, out NetworkObject target)
                    ? target
                    : null;

                if (Target != null)
                    StartFire();
            }

            if (_delayTimestamp.Expired(Runner))
            {
                if (Target != null && CanFire())
                {
                    RotateTowardsTarget();
                    Current.Fire();
                }

                _delayTimestamp = default;
            }
        }

        public override void Render()
        {
            while (_localFireStartedEvents < _fireStartedEvents)
            {
                OnFireStarted?.Invoke();
                _localFireStartedEvents++;
            }
        }
        
        private void RotateTowardsTarget()
        {
            Vector3 direction = Target.transform.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > MinRotationDirectionSqrMagnitude)
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        
        private bool CanFire()
        {
            Weapon current = Current;
            return current != null && current.CanFire() && (_condition == null || _condition.IsMet());
        }
        
        private void StartFire()
        {
            if (!_delayTimestamp.IsRunning && CanFire())
            {
                if (Target == null)
                    return;

                _delayTimestamp = TickTimer.CreateFromSeconds(Runner, _rangeFireDelay);
                _fireStartedEvents++;
            }
        }
    }
}
