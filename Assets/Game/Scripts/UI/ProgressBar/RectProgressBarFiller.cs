using UnityEngine;

namespace Game
{
    public class RectProgressBarFiller : ProgressBarFiller
    {
        [SerializeField] private RectTransform _rectTransform;

        public override float FillAmount
        {
            get => _rectTransform.anchorMax.x;
            set
            {
                var anchorMax = _rectTransform.anchorMax;
                anchorMax.x = value;
                _rectTransform.anchorMax = anchorMax;
            }
        }
    }
}