using Fusion;
using Game.Scripts.GameObjects;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.GameSystems.Economy;
using UnityEngine;

namespace Game.Scripts.GameSystems.Enemy
{
    public sealed class EnemySpawner : SimulationBehaviour
    {
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private SpawnPointService _spawnPointService;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private TeamWallet _teamWallet;
        [SerializeField] private GameState _gameState;

        private float _timer;

        public override void FixedUpdateNetwork()
        {
            // Object == null - сетевой объект сцены ещё не заспавнен, [Networked] читать нельзя.
            if (!Runner.IsServer || _gameState.Object == null || _gameState.IsGameOver)
                return;

            _timer += Runner.DeltaTime;
            if (_timer < _spawnInterval)
                return;

            _timer -= _spawnInterval;

            Transform spawnPoint = _spawnPointService.GetRandomSpawnPoint();
            Runner.Spawn(_enemyConfig.Prefab, spawnPoint.position, spawnPoint.rotation,
                onBeforeSpawned: (_, enemy) =>
                {
                    if (enemy.TryGetBehaviour(out TeamComponent team))
                        team.SetTeam(Team.Enemies);

                    if (enemy.TryGetBehaviour(out ContactDamageComponent contactDamage))
                        contactDamage.Init(_enemyConfig.Damage, _enemyConfig.DamageCooldown);

                    SubscribeReward(enemy);
                });
        }

        private void SubscribeReward(NetworkObject enemy)
        {
            if (_teamWallet == null || !enemy.TryGetBehaviour(out HealthComponent health))
                return;

            Vector2Int reward = _enemyConfig.Reward;
            health.OnDied += () => _teamWallet.AddCoins(Random.Range(reward.x, reward.y + 1));
        }
    }
}
