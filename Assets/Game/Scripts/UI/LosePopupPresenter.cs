using Game.Scripts.GameSystems.Player;
using UnityEngine;

namespace Game.Scripts.UI
{
    public sealed class LosePopupPresenter : MonoBehaviour
    {
        [SerializeField]
        private LoseNotificator _loseNotificator;

        [SerializeField]
        private GameObject _losePopup;

        private void OnEnable()
        {
            _loseNotificator.OnLose += OnLose;
        }

        private void OnDisable()
        {
            _loseNotificator.OnLose -= OnLose;
        }

        private void OnLose()
        {
            _losePopup.SetActive(true);
        }
    }
}
