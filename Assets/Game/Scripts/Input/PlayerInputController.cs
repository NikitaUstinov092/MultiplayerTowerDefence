using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerInputController : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject _character;

        public NetworkObject Character => _character;

        public override void FixedUpdateNetwork()
        {
            if (this.GetInput(out PlayerInputData inputData))
            {
                this.ProcessMove(inputData.MoveDirection);
            }
            else
            {
                this.StopMove();
            }
        }

        private void ProcessMove(Vector2 inputDirection)
        {
            MoveComponent moveComponent = _character.GetBehaviour<MoveComponent>();
            Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
            moveComponent.Move(moveDirection);
        }

        private void StopMove()
        {
            _character.GetBehaviour<MoveComponent>().Stop();
        }
    }
}