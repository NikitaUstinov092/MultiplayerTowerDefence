using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerJoinController : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private GameObject _characterPrefab;

        void IPlayerJoined.PlayerJoined(PlayerRef player)
        {
            if (this.Runner.IsServer)
            {
                NetworkObject character = this.Runner.Spawn(_characterPrefab, Vector3.zero, Quaternion.identity, player);
                this.Runner.SetPlayerObject(player, character); // Index peer, NetworkId
            }
        }
    }
}