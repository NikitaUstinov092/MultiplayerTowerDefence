using Fusion;
using Game.Scripts.GameObjects.Core;
using UnityEngine;

namespace Game.Scripts.GameObjects.View
{
    public sealed class MoveAnimComponent : NetworkBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash(nameof(IsMoving));

        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private Animator _animator;

        public override void Spawned()
        {
            _moveComponent.OnStateChanged += this.OnStateChanged;
            this.OnStateChanged();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _moveComponent.OnStateChanged -= this.OnStateChanged;
        }

        private void OnStateChanged()
        {
            _animator.SetBool(IsMoving, _moveComponent.IsMoving);
        }
    }
}