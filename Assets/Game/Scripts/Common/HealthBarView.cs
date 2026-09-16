using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame
{
    public sealed class HealthBarView : MonoBehaviour
    {
		[SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Image _progress;

        public void SetText(string health)
        {
            _text.text = health;
        }

        public void SetProgress(float progress)
        {
            _progress.fillAmount = Mathf.Clamp01(progress);
        }
    }
}
