using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    [DisallowMultipleComponent]
    public class NetworkObjectProviderPool : NetworkObjectProviderDefault
    {
        [SerializeField]
        private Transform _container;

        [SerializeField, Min(0)]
        private int _maxPoolCount = 32;

        [SerializeField, Min(0)]
        private int _initialStackSize = 4;

        private readonly Dictionary<NetworkObject, Stack<NetworkObject>> _freeObjects = new();
        private readonly Dictionary<NetworkObject, NetworkObject> _rentVsPrefab = new();

        protected sealed override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
        {
            if (!prefab)
                return null;

            Stack<NetworkObject> stack = this.GetOrCreateStack(prefab);
            while (stack.Count > 0)
            {
                NetworkObject instance = stack.Pop();
                if (!instance)
                    continue;

                instance.transform.SetParent(null, false);
                instance.gameObject.SetActive(true);

                _rentVsPrefab.Add(instance, prefab);
                return instance;
            }

            NetworkObject created = this.InstantiatePrefab(prefab);
            _rentVsPrefab.Add(created, prefab);
            return created;
        }

        protected virtual NetworkObject InstantiatePrefab(NetworkObject prefab)
        {
            return Instantiate(prefab);
        }

        protected override void DestroyPrefabInstance(
            NetworkRunner runner,
            NetworkPrefabId prefabId,
            NetworkObject instance
        )
        {
            if (!instance)
                return;

            if (!_rentVsPrefab.Remove(instance, out NetworkObject prefab))
            {
                Destroy(instance.gameObject);
                return;
            }

            Stack<NetworkObject> stack = this.GetOrCreateStack(prefab);
            if (_maxPoolCount > 0 && stack.Count >= _maxPoolCount)
            {
                Destroy(instance.gameObject);
                return;
            }

            instance.gameObject.SetActive(false);

            if (_container)
            {
                instance.transform.SetParent(_container, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;
            }
            
            stack.Push(instance);
        }

        private Stack<NetworkObject> GetOrCreateStack(NetworkObject prefab)
        {
            if (_freeObjects.TryGetValue(prefab, out Stack<NetworkObject> stack))
                return stack;

            stack = new Stack<NetworkObject>(Mathf.Max(0, _initialStackSize));
            _freeObjects.Add(prefab, stack);

            return stack;
        }

        private void OnDestroy()
        {
            foreach (Stack<NetworkObject> stack in _freeObjects.Values)
            {
                while (stack.Count > 0)
                {
                    NetworkObject instance = stack.Pop();
                    if (instance)
                        Destroy(instance.gameObject);
                }
            }

            _freeObjects.Clear();
            _rentVsPrefab.Clear();
        }
    }
}