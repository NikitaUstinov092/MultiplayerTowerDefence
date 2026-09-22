using Fusion;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Purchasable", order = 0)]
    public sealed class PurchasableConfig : ScriptableObject
    {
        [SerializeField] private NetworkPrefabRef _prefab;
        [SerializeField] private int _price = 100;

        public NetworkPrefabRef Prefab => _prefab;
        public int Price => _price;
    }
}
