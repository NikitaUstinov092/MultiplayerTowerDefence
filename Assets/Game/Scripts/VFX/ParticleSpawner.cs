using Game.Scripts.Common;
using UnityEngine;

namespace Game.Scripts.VFX
{
    public sealed class ParticleSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _playPoint;

        public void Play()
        {
            if (ServiceLocator.TryGet(out ParticlePool pool))
                pool.Play(_effectPrefab, _playPoint.position, _playPoint.rotation);
            else
                Instantiate(_effectPrefab, _playPoint.position, _playPoint.rotation, null);
        }
    }
}
