using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class SmoothHealthBar : MonoBehaviour
    {
        [SerializeField] private ProgressBarFiller _instantFiller;
        [SerializeField] private ProgressBarFiller _followFiller;
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private float _smoothDuration = 0.3f;
        
        private Tween _smoothAnimation;

        public void SetCaption(string caption)
        {
            _caption.text = caption;
        }
        
        public void Set(float normalizedHealth, bool smoothFollow)
        {
            _instantFiller.FillAmount = normalizedHealth;

            if (_smoothAnimation.IsActive())
            {
                _smoothAnimation.Kill();
            }
            
            if (smoothFollow)
            {
                _smoothAnimation = DOVirtual.Float(_followFiller.FillAmount,
                        normalizedHealth,
                        _smoothDuration,
                        value => _followFiller.FillAmount = value)
                    .SetLink(_followFiller.gameObject);
            }
            else
            {
                _followFiller.FillAmount = normalizedHealth;
            }
        }
    }
}