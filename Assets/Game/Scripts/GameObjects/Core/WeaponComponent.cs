using System;
using Fusion;
using UnityEngine;

namespace SampleGame
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
            if (this.HasStateAuthority && !_delayTimestamp.IsRunning)
            {
                this.Target = this.CanFire() &&
                              NearestEnemyFinder.TryFind(this.Runner, this.transform.position, _detectionRadius, _detectionLayerMask, this.Object, out NetworkObject target)
                    ? target
                    : null;

                if (this.Target != null)
                    this.StartFire();
            }

            if (_delayTimestamp.Expired(this.Runner))
            {
                // Если условие пропало за время замаха (двинулся, умер) - замах отменяется. Иначе истёкший
                // таймер остаётся IsRunning, блокирует поиск цели и выстрел срабатывает позже по устаревшей цели.
                if (this.Target != null && this.CanFire())
                {
                    this.RotateTowardsTarget();
                    this.Current.Fire();
                }

                _delayTimestamp = default;
            }
        }

        public override void Render()
        {
            while (_localFireStartedEvents < _fireStartedEvents)
            {
                this.OnFireStarted?.Invoke();
                _localFireStartedEvents++;
            }
        }
        
        private void RotateTowardsTarget()
        {
            Vector3 direction = this.Target.transform.position - this.transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > MinRotationDirectionSqrMagnitude)
                this.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        
        private bool CanFire()
        {
            Weapon current = this.Current;
            return current != null && current.CanFire() && (_condition == null || _condition.IsMet());
        }
        
        private void StartFire()
        {
            if (!_delayTimestamp.IsRunning && this.CanFire())
            {
                if (this.Target == null)
                    return;

                _delayTimestamp = TickTimer.CreateFromSeconds(this.Runner, _rangeFireDelay);
                _fireStartedEvents++;
            }
        }
    }
}
