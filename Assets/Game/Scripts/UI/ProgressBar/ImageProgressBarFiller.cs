using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.ProgressBar
{
    public class ImageProgressBarFiller : ProgressBarFiller
    {
        [SerializeField] private Image _image;

        public override float FillAmount
        {
            get => _image.fillAmount;
            set => _image.fillAmount = value;
        }
    }
}