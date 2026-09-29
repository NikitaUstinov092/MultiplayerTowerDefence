using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.VFX
{
    public sealed class ParticlePool : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;

        private readonly Dictionary<ParticleSystem, Stack<PooledParticle>> _available = new();

        public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            Stack<PooledParticle> stack = GetStack(prefab);

            PooledParticle instance = stack.Count > 0 ? stack.Pop() : Create(prefab);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            instance.System.Play(withChildren: true);
        }

        public void Release(PooledParticle instance)
        {
            instance.gameObject.SetActive(false);
            GetStack(instance.Prefab).Push(instance);
        }

        private Stack<PooledParticle> GetStack(ParticleSystem prefab)
        {
            if (!_available.TryGetValue(prefab, out Stack<PooledParticle> stack))
            {
                stack = new Stack<PooledParticle>();
                _available.Add(prefab, stack);
            }

            return stack;
        }

        private PooledParticle Create(ParticleSystem prefab)
        {
            Transform parent = _container != null ? _container : transform;
            ParticleSystem vfx = Instantiate(prefab, parent);
            vfx.name = prefab.name;

            if (!vfx.TryGetComponent(out PooledParticle instance))
                instance = vfx.gameObject.AddComponent<PooledParticle>();

            instance.Init(this, prefab);
            return instance;
        }
    }
}
