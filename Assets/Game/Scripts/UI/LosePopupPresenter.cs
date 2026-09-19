using SampleGame;
using UnityEngine;

namespace Game
{
    public sealed class LosePopupPresenter : MonoBehaviour
    {
        [SerializeField]
        private LoseNotificator _loseNotificator;

        [SerializeField]
        private GameObject _losePopup;

        private void OnEnable()
        {
            _loseNotificator.OnLose += this.OnLose;
        }

        private void OnDisable()
        {
            _loseNotificator.OnLose -= this.OnLose;
        }

        private void OnLose()
        {
            _losePopup.SetActive(true);
        }
    }
}
