using UnityEngine;

namespace Game
{
    public sealed class ParticleSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _playPoint;

        public void Play()
        {
            ParticlePool pool = ParticlePool.Instance;
            if (pool != null)
                pool.Play(_effectPrefab, _playPoint.position, _playPoint.rotation);
            else
                Instantiate(_effectPrefab, _playPoint.position, _playPoint.rotation, null);
        }
    }
}
