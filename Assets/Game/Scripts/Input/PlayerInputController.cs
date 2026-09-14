using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerInputController : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject _character;

        [Networked]
        private NetworkButtons _previousButtons { get; set; }
        
        public override void FixedUpdateNetwork() 
        {
            if (this.GetInput(out PlayerInputData inputData))
            {
                NetworkButtons inputButtons = inputData.buttons;
                this.ProcessMove(inputData.moveDirection);
                this.ProcessSprint(inputButtons);
                this.ProcessFire(inputButtons);
                _previousButtons = inputButtons;
            }
            else
            {
                this.StopMove();
            }
        }

        private void ProcessSprint(NetworkButtons inputButtons)
        {
            bool sprint = inputButtons.IsSet(PlayerInputButtons.Sprint);
            _character.GetBehaviour<SprintComponent>().IsSprint = sprint;
        }

        private void ProcessFire(NetworkButtons inputButtons)
        {
            if (inputButtons.WasPressed(_previousButtons, PlayerInputButtons.Fire)) 
                _character.GetBehaviour<WeaponComponent>().StartFire();
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