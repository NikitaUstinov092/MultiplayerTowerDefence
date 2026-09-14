using Fusion;

namespace SampleGame
{
    public sealed class PlayerLeftController : SimulationBehaviour, IPlayerLeft
    {
        void IPlayerLeft.PlayerLeft(PlayerRef player)
        {
            if (this.Runner.IsServer && this.Runner.TryGetPlayerObject(player, out NetworkObject character)) 
                this.Runner.Despawn(character);
        }
    }
}