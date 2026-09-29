using UnityEngine;

namespace Game.Scripts.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class PooledParticle : MonoBehaviour
    {
        private ParticlePool _pool;

        public ParticleSystem System { get; private set; }
        public ParticleSystem Prefab { get; private set; }

        public void Init(ParticlePool pool, ParticleSystem prefab)
        {
            _pool = pool;
            Prefab = prefab;
            System = GetComponent<ParticleSystem>();

            // Unity вызывает OnParticleSystemStopped только при stopAction = Callback,
            // выставляем в коде, чтобы не настраивать каждый префаб вручную.
            ParticleSystem.MainModule main = System.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            _pool.Release(this);
        }
    }
}
