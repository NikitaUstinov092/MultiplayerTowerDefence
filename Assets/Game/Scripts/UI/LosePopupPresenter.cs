using Game.Scripts.GameSystems;
using UnityEngine;

namespace Game.Scripts.UI
{
    public sealed class LosePopupPresenter : MonoBehaviour
    {
        [SerializeField]
        private GameState _gameState;

        [SerializeField]
        private GameObject _losePopup;

        private void OnEnable()
        {
            _gameState.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            _gameState.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            _losePopup.SetActive(true);
        }
    }
}
