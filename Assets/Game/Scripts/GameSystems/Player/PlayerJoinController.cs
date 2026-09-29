using Fusion;
using Game.Scripts.GameObjects;
using Game.Scripts.GameObjects.Core;
using UnityEngine;

namespace Game.Scripts.GameSystems.Player
{
    public sealed class PlayerJoinController : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private SpawnPointService _spawnPointService;

        void IPlayerJoined.PlayerJoined(PlayerRef player)
        {
            if (Runner.IsServer)
            {
                Transform spawnPoint = _spawnPointService.GetSpawnPoint(player.AsIndex % _spawnPointService.Count);
                NetworkObject character = Runner.Spawn(_characterPrefab, spawnPoint.position, spawnPoint.rotation, player);
                Runner.SetPlayerObject(player, character); // Index peer, NetworkId

                if (character.TryGetBehaviour(out TeamComponent team))
                    team.SetTeam(Team.Players);
            }
        }
    }
}