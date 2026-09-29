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

        private float _timer;

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer || IsAnyPlayerDead())
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
        
        private bool IsAnyPlayerDead()
        {
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                if (!Runner.TryGetPlayerObject(player, out NetworkObject playerObject))
                    continue;

                HealthComponent health = playerObject.GetComponentInChildren<HealthComponent>();
                if (health != null && health.IsDead)
                    return true;
            }

            return false;
        }
    }
}
