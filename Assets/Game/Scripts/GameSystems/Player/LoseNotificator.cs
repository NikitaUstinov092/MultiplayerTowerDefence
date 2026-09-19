using System;
using Fusion;

namespace SampleGame
{
    public sealed class LoseNotificator : SimulationBehaviour
    {
        public event Action OnLose;

        public void NotifyAboutLose()
        {
            if (this.Runner.IsServer)
                RpcLose(this.Runner);
        }

        // Server -> Client
        [Rpc(
            InvokeLocal = true,
            TickAligned = false,
            Channel = RpcChannel.Reliable,
            HostMode = RpcHostMode.SourceIsServer
        )]
        private static void RpcLose(NetworkRunner runner)
        {
            LoseNotificator notificator = runner.GetBehaviour<LoseNotificator>();
            notificator.OnLose?.Invoke();
        }
    }
}
