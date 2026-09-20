using System;
using Fusion;
using UnityEngine;

namespace SampleGame
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

        public bool IsMoving => this.MoveDirection != Vector3.zero;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        [Networked, OnChangedRender(nameof(MoveDirectionChanged))]
        public Vector3 MoveDirection { get; private set; }
        
        // FUN
        public void Move(Vector3 direction)
        {
            this.MoveDirection = _condition == null || _condition.IsMet() ? direction : Vector3.zero;

            if (this.MoveDirection != Vector3.zero)
            {
                float deltaTime = Time.fixedDeltaTime;
                this.UpdateRotation(this.MoveDirection, deltaTime);
                this.UpdatePosition(this.MoveDirection, deltaTime);
            }
        }

        // FUN
        public void Stop()
        {
            this.MoveDirection = Vector3.zero;
        }

        private void UpdateRotation(Vector3 direction, float deltaTime)
        {
            Quaternion current = this.transform.rotation;
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            this.transform.rotation = Quaternion.RotateTowards(current, target, _angularSpeed * deltaTime);
        }

        private void UpdatePosition(Vector3 direction, float deltaTime)
        {
            this.transform.position += direction * deltaTime * _moveSpeed;
        }

        private void MoveDirectionChanged()
        {
            this.OnStateChanged?.Invoke();
        }
    }
}

// // Forecast Physics
// private void FixedUpdate()
// {
//     if (!this.StateBufferIsValid)
//         return;
//     
//     Vector3 moveDirection = this.MoveDirection;
//     if (moveDirection != Vector3.zero && (_condition == null || _condition.IsMet()))
//     {
//         float deltaTime = Time.fixedDeltaTime;
//         this.UpdateRotation(moveDirection, deltaTime);
//         this.UpdatePosition(moveDirection, deltaTime);
//     }
// }
