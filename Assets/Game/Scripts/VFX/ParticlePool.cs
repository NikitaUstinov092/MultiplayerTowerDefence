using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class ParticlePool : MonoBehaviour
    {
        // Чистый визуал вне симуляции - сетевой runner не нужен, достаточно одного пула на сцену.
        public static ParticlePool Instance { get; private set; }

        [SerializeField]
        private Transform _container;

        private readonly Dictionary<ParticleSystem, Stack<ParticleSystem>> _available = new();
        private readonly List<(ParticleSystem instance, ParticleSystem prefab)> _playing = new();

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            if (!_available.TryGetValue(prefab, out Stack<ParticleSystem> stack))
            {
                stack = new Stack<ParticleSystem>();
                _available.Add(prefab, stack);
            }

            ParticleSystem instance = stack.Count > 0 ? stack.Pop() : Create(prefab);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            instance.Play(withChildren: true);

            _playing.Add((instance, prefab));
        }

        private void Update()
        {
            for (int i = _playing.Count - 1; i >= 0; i--)
            {
                (ParticleSystem instance, ParticleSystem prefab) = _playing[i];
                if (instance.IsAlive(withChildren: true))
                    continue;

                instance.gameObject.SetActive(false);
                _available[prefab].Push(instance);
                _playing.RemoveAt(i);
            }
        }

        private ParticleSystem Create(ParticleSystem prefab)
        {
            Transform parent = _container != null ? _container : this.transform;
            ParticleSystem instance = Instantiate(prefab, parent);
            instance.name = prefab.name;
            return instance;
        }
    }
}
