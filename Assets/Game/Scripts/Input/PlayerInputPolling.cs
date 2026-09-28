using Fusion;
using UnityEngine;

namespace Game.Scripts.Input
{
    public sealed class PlayerInputPolling : MonoBehaviour
    {
        [SerializeField] private NetworkEvents _networkEvents;
        [SerializeField] private PlayerInputMap _inputMap;

        private PlayerInputData _currentInput;

        private void Update()
        {
            if (!this.HasWindowFocus())
            {
                _currentInput = default;
                return;
            }

            _currentInput.MoveDirection = _inputMap.GetMoveDirection();
            _currentInput.Buttons.Set(PlayerInputButtons.BuyMine, _inputMap.IsBuyMine());
            _currentInput.Buttons.Set(PlayerInputButtons.BuyArcher, _inputMap.IsBuyArcher());
        }

        private void OnEnable() =>
            _networkEvents.OnInput.AddListener(this.OnInput);

        private void OnDisable() =>
            _networkEvents.OnInput.RemoveListener(this.OnInput);

        private void OnInput(NetworkRunner runner, NetworkInput input) =>
            input.Set(_currentInput);

        private bool HasWindowFocus()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isFocused && Application.isFocused;
#else
            return Application.isFocused;
#endif
        }
    }
}

