using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerJoinController : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private SpawnPointService _spawnPointService;

        void IPlayerJoined.PlayerJoined(PlayerRef player)
        {
            if (this.Runner.IsServer)
            {
                Transform spawnPoint = _spawnPointService.GetSpawnPoint(player.AsIndex % _spawnPointService.Count);
                NetworkObject character = this.Runner.Spawn(_characterPrefab, spawnPoint.position, spawnPoint.rotation, player);
                this.Runner.SetPlayerObject(player, character); // Index peer, NetworkId

                if (character.TryGetBehaviour(out TeamComponent team))
                    team.SetTeam(Team.Players);
            }
        }
    }
}