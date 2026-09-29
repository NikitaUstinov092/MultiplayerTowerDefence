using Fusion;
using UnityEngine;

namespace Game.Scripts.GameSystems.Enemy
{
    [CreateAssetMenu(menuName = "Game/Enemy",order = 0)]
    public sealed class EnemyConfig : ScriptableObject
    {
        [SerializeField] private NetworkPrefabRef _prefab;
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _playerDamageCooldown = 1f;
        [SerializeField] private Vector2Int _reward;

        public NetworkPrefabRef Prefab => _prefab;
        public int Damage => _damage;
        public float DamageCooldown => _playerDamageCooldown;
        public Vector2Int Reward => _reward;
    }
}