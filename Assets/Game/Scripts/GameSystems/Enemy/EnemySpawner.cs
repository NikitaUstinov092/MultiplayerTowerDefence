using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
{
    public sealed class EnemySpawner : SimulationBehaviour
    {
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private SpawnPointService _spawnPointService;
        [SerializeField] private float _spawnInterval = 5f;

        private float _timer;

        public override void FixedUpdateNetwork()
        {
            if (!this.Runner.IsServer || this.IsAnyPlayerDead())
                return;

            _timer += this.Runner.DeltaTime;
            if (_timer < _spawnInterval)
                return;

            _timer -= _spawnInterval;

            Transform spawnPoint = _spawnPointService.GetRandomSpawnPoint();
            this.Runner.Spawn(_enemyConfig.Prefab, spawnPoint.position, spawnPoint.rotation);
        }

        /// <summary>
        /// Здесь бегу по всем игрокам, хотя руки тянутся привязаться к событию.
        /// Но Игорь говорит что так делать нельзя, потому что события срабатывают в OnRender
        /// </summary>
        /// <returns></returns>
        private bool IsAnyPlayerDead()
        {
            foreach (PlayerRef player in this.Runner.ActivePlayers)
            {
                if (!this.Runner.TryGetPlayerObject(player, out NetworkObject playerObject))
                    continue;

                HealthComponent health = playerObject.GetComponentInChildren<HealthComponent>();
                if (health != null && health.IsDead)
                    return true;
            }

            return false;
        }
    }
}
