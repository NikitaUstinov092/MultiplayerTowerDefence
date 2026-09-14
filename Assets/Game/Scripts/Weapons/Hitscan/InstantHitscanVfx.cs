using UnityEngine;

namespace SampleGame
{
    public sealed class InstantHitscanVfx : MonoBehaviour
    {
        [SerializeField, Min(0.01f)]
        private float _lifetime = 2f;

        [Header("Trail")]
        [SerializeField]
        private LineRenderer _trail;

        [SerializeField]
        private Gradient _fadeoutGradient;

        [SerializeField]
        private float _fadeoutWidthMultiplier = 0.13f;

        private float _startWidthMultiplier;
        private Gradient _startGradient;
        private Gradient _trailGradient;

        private GradientColorKey[] _startColorKeys;
        private GradientColorKey[] _fadeoutColorKeys;
        private GradientColorKey[] _resultColorKeys;

        private GradientAlphaKey[] _startAlphaKeys;
        private GradientAlphaKey[] _fadeoutAlphaKeys;
        private GradientAlphaKey[] _resultAlphaKeys;

        private float _time;

        private void Awake()
        {
            _startWidthMultiplier = _trail.widthMultiplier;
            _startGradient = _trail.colorGradient;

            _startColorKeys = _startGradient.colorKeys;
            _fadeoutColorKeys = _fadeoutGradient.colorKeys;

            _startAlphaKeys = _startGradient.alphaKeys;
            _fadeoutAlphaKeys = _fadeoutGradient.alphaKeys;

            if (_startColorKeys.Length != _fadeoutColorKeys.Length ||
                _startAlphaKeys.Length != _fadeoutAlphaKeys.Length)
            {
                Debug.LogError(
                    $"{name} - Trail gradient and fadeout gradient must have " +
                    "identical number of color and alpha keys.",
                    this
                );

                enabled = false;
                return;
            }

            _resultColorKeys = new GradientColorKey[_startColorKeys.Length];
            _resultAlphaKeys = new GradientAlphaKey[_startAlphaKeys.Length];

            _trailGradient = new Gradient();

            _trail.enabled = false;
            enabled = false;
        }

        public void Play(Vector3 start, Vector3 end)
        {
            _trail.positionCount = 2;
            _trail.SetPosition(0, start);
            _trail.SetPosition(1, end);
            
            _trail.colorGradient = _startGradient;
            _trail.widthMultiplier = _startWidthMultiplier;

            _time = 0f;

            _trail.enabled = true;
            enabled = true;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            float progress = Mathf.Clamp01(_time / _lifetime);

            UpdateGradient(progress);

            _trail.colorGradient = _trailGradient;
            _trail.widthMultiplier = Mathf.Lerp(
                _startWidthMultiplier,
                _fadeoutWidthMultiplier,
                progress
            );

            if (progress >= 1f)
            {
                _trail.enabled = false;
                enabled = false;
            }
        }

        private void UpdateGradient(float progress)
        {
            for (int i = 0; i < _resultColorKeys.Length; i++)
            {
                _resultColorKeys[i].color = Color.Lerp(
                    _startColorKeys[i].color,
                    _fadeoutColorKeys[i].color,
                    progress
                );

                _resultColorKeys[i].time = Mathf.Lerp(
                    _startColorKeys[i].time,
                    _fadeoutColorKeys[i].time,
                    progress
                );
            }

            for (int i = 0; i < _resultAlphaKeys.Length; i++)
            {
                _resultAlphaKeys[i].alpha = Mathf.Lerp(
                    _startAlphaKeys[i].alpha,
                    _fadeoutAlphaKeys[i].alpha,
                    progress
                );

                _resultAlphaKeys[i].time = Mathf.Lerp(
                    _startAlphaKeys[i].time,
                    _fadeoutAlphaKeys[i].time,
                    progress
                );
            }

            _trailGradient.SetKeys(_resultColorKeys, _resultAlphaKeys);
        }
    }
}