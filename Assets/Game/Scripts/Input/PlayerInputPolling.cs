using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerInputPolling : MonoBehaviour
    {
        [SerializeField] private NetworkEvents _networkEvents;
        [SerializeField] private PlayerInputMap _inputMap;

        private PlayerInputData _currentInput;
        private bool _resetInputs;

        private void Update()
        {
            if (_resetInputs)
            {
                _currentInput.buttons.Set(PlayerInputButtons.Fire, false);
                _resetInputs = false;
            }
            
            _currentInput.moveDirection = _inputMap.GetMoveDirection();
            _currentInput.buttons.Set(PlayerInputButtons.Sprint, _inputMap.IsSprint());
            
            if (_inputMap.IsFirePressed()) 
                _currentInput.buttons.Set(PlayerInputButtons.Fire, true);
        }

        private void OnEnable() =>
            _networkEvents.OnInput.AddListener(this.OnInput);

        private void OnDisable() =>
            _networkEvents.OnInput.RemoveListener(this.OnInput);

        private void OnInput(NetworkRunner runner, NetworkInput input)
        {
            input.Set(_currentInput);
            _resetInputs = true;
        }
    }
}

