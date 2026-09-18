using System;
using System.Collections.Generic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace SampleGame
{
    public sealed class WeaponComponent : NetworkBehaviour
    {
        private const float f = 0.0001f;
        
        public interface ICondition
        {
            bool IsMet();
        }

        public event Action OnFireStarted;

        [Networked, UnitySerializeField]
        public Weapon Current { get; private set; }

        [Networked]
        public NetworkObject Target { get; set; }

        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private float _detectionRadius = 15f;

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

        public bool IsFireStarted => _delayTimestamp.IsRunning(this.Runner);

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        [Button]
        public void SetTarget(NetworkObject target)
        {
            if (this.HasStateAuthority)
                this.Target = target;
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

            if (direction.sqrMagnitude > f)
                this.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private bool FindClosestTarget(out NetworkObject target)
        {
            List<NetworkObject> buffer = ListPool<NetworkObject>.Get();
            this.Runner.GetAllNetworkObjects(buffer);

            PlayerRef player = this.Object.InputAuthority;
            float detectionRadius = _detectionRadius * _detectionRadius;
            float minDistance = detectionRadius;
            target = null;

            for (int i = 0, count = buffer.Count; i < count; i++)
            {
                NetworkObject obj = buffer[i];
                if (!obj.TryGetBehaviour(out HealthComponent health) || !health.IsAlive || !health.CanBeDamagedBy(player))
                    continue;

                float distance = (obj.transform.position - this.transform.position).sqrMagnitude;
                if (distance > minDistance)
                    continue;

                minDistance = distance;
                target = obj;
            }

            ListPool<NetworkObject>.Release(buffer);

            return target != null;
        }

        // Kiss
        private float GetDelay()
        {
            Weapon current = this.Current;
            if (current is MeleeWeapon)
                return _meleeFireDelay;

            if (current is ProjectileWeapon or HitscanWeapon)
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
            if (!_delayTimestamp.IsRunning)
            {
                this.Target = this.CanFire() && !_moveComponent.IsMoving && this.FindClosestTarget(out NetworkObject target)
                    ? target
                    : null;

                if (this.Target != null)
                    this.StartFire();
            }

            if (_delayTimestamp.Expired(this.Runner) && this.CanFire())
            {
                if (this.Target != null)
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