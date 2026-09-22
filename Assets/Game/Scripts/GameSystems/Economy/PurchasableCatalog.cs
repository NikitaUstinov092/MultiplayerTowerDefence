using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(
        fileName = "PurchasableCatalog",
        menuName = "Game/Economy/New PurchasableCatalog"
    )]
    public sealed class PurchasableCatalog : ScriptableObject,
        IReadOnlyCollection<KeyValuePair<PlayerKeys, PurchasableConfig>>
    {
        [SerializeField]
        private SerializableDictionary<PlayerKeys, PurchasableConfig> _configs = new();

        public int Count => _configs.Count;

        public bool TryGetConfig(PlayerKeys id, out PurchasableConfig config) =>
            _configs.TryGetValue(id, out config);

        public IEnumerator<KeyValuePair<PlayerKeys, PurchasableConfig>> GetEnumerator() => _configs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
