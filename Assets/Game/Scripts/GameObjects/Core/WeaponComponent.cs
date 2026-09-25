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

        public interface IIdleCondition
        {
            bool IsMet();
        }

        public event Action OnFireStarted;

        [Networked, UnitySerializeField]
        public Weapon Current { get; private set; }

        [Networked]
        public NetworkObject Target { get; set; }

        [SerializeField]
        private float _detectionRadius = 15f;

        [SerializeField]
        private LayerMask _detectionLayerMask;

        [Header("Weapon delay")]
        [SerializeField]
        private float _meleeFireDelay = 0.25f;

        [SerializeField]
        private float _rangeFireDelay = 0.2f;

        [SerializeField]
        private float _grenadeFireDelay = 1f;

        [Networked]
        private TickTimer _delayTimestamp { get; set; }

        [Networked]
        private ushort _fireStartedEvents { get; set; }

        private ushort _localFireStartedEvents;

        private ICondition _condition;
        private IIdleCondition _idleCondition;

        public bool IsFireStarted => _delayTimestamp.IsRunning(this.Runner);

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        public void SetIdleCondition(IIdleCondition condition)
        {
            _idleCondition = condition;
        }

        public void StartFire()
        {
            if (!_delayTimestamp.IsRunning && this.CanFire())
            {
                if (this.Target == null)
                    return;

                _delayTimestamp = TickTimer.CreateFromSeconds(this.Runner, this.GetDelay());
                _fireStartedEvents++;
            }
        }

        private void RotateTowardsTarget()
        {
            Vector3 direction = this.Target.transform.position - this.transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > MinRotationDirectionSqrMagnitude)
                this.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        // Kiss
        private float GetDelay()
        {
            Weapon current = this.Current;
            if (current is MeleeWeapon)
                return _meleeFireDelay;

            if (current is ProjectileWeapon)
                return _rangeFireDelay;

            if (current is GrenadeWeapon)
                return _grenadeFireDelay;

            throw new Exception($"Undefined weapon type {current.GetType().Name}!");
        }

        public bool CanFire()
        {
            Weapon current = this.Current;
            return current != null && current.CanFire() && (_condition == null || _condition.IsMet());
        }

        public override void Spawned()
        {
            _localFireStartedEvents = _fireStartedEvents;
        }

        public override void FixedUpdateNetwork()
        {
            // Поиск/прицел/выстрел - решение сервера. Без этой проверки тот же код выполняется
            // ещё раз на клиенте-владельце (у него InputAuthority), удваивая результат.
            if (!this.HasStateAuthority)
                return;

            if (!_delayTimestamp.IsRunning)
            {
                bool canSearch = this.CanFire() && (_idleCondition == null || _idleCondition.IsMet());
                this.Target = canSearch &&
                              NearestEnemyFinder.TryFind(this.Runner, this.transform.position, _detectionRadius, _detectionLayerMask, this.Object, out NetworkObject target)
                    ? target
                    : null;

                if (this.Target != null)
                    this.StartFire();
            }

            if (_delayTimestamp.Expired(this.Runner) && this.CanFire())
            {
                // Цель могла умереть/деспавниться за время задержки выстрела - Target тогда
                // резолвится в null. Стрелять по устаревшему направлению прицела нельзя.
                if (this.Target == null)
                {
                    _delayTimestamp = default;
                    return;
                }

                this.RotateTowardsTarget();
                this.Current.Fire();
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
    }
}