using UnityEngine;

namespace Game
{
    public sealed class SpawnPointService : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;

        public int Count => _spawnPoints.Length;

        public Transform GetSpawnPoint(int index)
        {
            return _spawnPoints[index];
        }

        public Transform GetRandomSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, Count)];
        }
    }
}