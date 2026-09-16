using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

namespace SampleGame
{
    public sealed class ProjectileViewPool : Behaviour
    {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private SerializableDictionary<ProjectileType, Pool> _pools = new();

        private readonly Dictionary<ProjectileView, Pool> _rented = new();

        public ProjectileView Rent(ProjectileType type, Transform parent)
        {
            if (!_pools.TryGetValue(type, out Pool pool))
                throw new ArgumentOutOfRangeException(nameof(type), type,
                    "Pool is not configured for this projectile type.");

            ProjectileView view = pool.Rent(parent);

            if (!_rented.TryAdd(view, pool))
                throw new InvalidOperationException($"Projectile '{view.name}' is already rented.");

            return view;
        }

        public void Return(ProjectileView view)
        {
            if (view == null)
                return;

            if (!_rented.Remove(view, out Pool pool))
            {
                Debug.LogWarning($"Projectile '{view.name}' was not rented from this pool.", view);
                return;
            }

            view.transform.SetParent(_container, false);
            pool.Return(view);
        }

        [Serializable]
        private sealed class Pool
        {
            [SerializeField]
            private ProjectileView _prefab;

            private readonly Stack<ProjectileView> _available = new();

            public ProjectileView Rent(Transform parent)
            {
                ProjectileView view = _available.Count > 0 ? _available.Pop() : Create(parent);
                view.transform.SetParent(parent, false);
                view.gameObject.SetActive(true);
                return view;
            }

            public void Return(ProjectileView view)
            {
                view.gameObject.SetActive(false);
                _available.Push(view);
            }

            private ProjectileView Create(Transform parent)
            {
                ProjectileView view = Instantiate(_prefab, parent);
                view.name = _prefab.name;
                return view;
            }
        }
    }
}