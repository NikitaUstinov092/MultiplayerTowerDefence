using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerInputPolling : MonoBehaviour
    {
        [SerializeField] private NetworkEvents _networkEvents;
        [SerializeField] private PlayerInputMap _inputMap;

        private PlayerInputData _currentInput;

        private void Update()
        {
            // Без фокуса окна Input.GetKey/GetAxis всё равно ловит нажатия с ОС (если включён Run In Background),
            // поэтому свёрнутый инстанс не должен подмешивать свой ввод.
            // Application.isFocused в редакторе отражает фокус Game View внутри процесса, а не фокус
            // самого процесса Editor на уровне ОС - между двумя клонами (ParrelSync) этого недостаточно,
            // поэтому дополнительно проверяем EditorApplication.isFocused.
            if (!this.HasWindowFocus())
            {
                _currentInput = default;
                return;
            }

            _currentInput.moveDirection = _inputMap.GetMoveDirection();
            _currentInput.buttons.Set(PlayerInputButtons.Sprint, _inputMap.IsSprint());
            _currentInput.buttons.Set(PlayerInputButtons.BuyMine, _inputMap.IsBuyMine());
            _currentInput.buttons.Set(PlayerInputButtons.BuyArcher, _inputMap.IsBuyArcher());
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

