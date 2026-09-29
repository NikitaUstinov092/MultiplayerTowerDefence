using System;
using Fusion;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class MoveComponent : NetworkBehaviour
    {
        public interface ICondition
        {
            public bool IsMet();
        }

        public event Action OnStateChanged;

        [SerializeField]
        private float _moveSpeed = 5;

        [SerializeField]
        private float _angularSpeed = 720;

        private ICondition _condition;

        public bool IsMoving => MoveDirection != Vector3.zero;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        [Networked, OnChangedRender(nameof(MoveDirectionChanged))]
        public Vector3 MoveDirection { get; private set; }
        
        // FUN
        public void Move(Vector3 direction)
        {
            MoveDirection = _condition == null || _condition.IsMet() ? direction : Vector3.zero;

            if (MoveDirection != Vector3.zero)
            {
                float deltaTime = Time.fixedDeltaTime;
                UpdateRotation(MoveDirection, deltaTime);
                UpdatePosition(MoveDirection, deltaTime);
            }
        }

        // FUN
        public void Stop()
        {
            MoveDirection = Vector3.zero;
        }

        private void UpdateRotation(Vector3 direction, float deltaTime)
        {
            Quaternion current = transform.rotation;
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(current, target, _angularSpeed * deltaTime);
        }

        private void UpdatePosition(Vector3 direction, float deltaTime)
        {
            transform.position += direction * deltaTime * _moveSpeed;
        }

        private void MoveDirectionChanged()
        {
            OnStateChanged?.Invoke();
        }
    }
}

