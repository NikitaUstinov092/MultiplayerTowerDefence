namespace Fusion
{
    public static class NetworkExtensions
    {
        public static void ReplicateOnlyToInputAuthority(this NetworkBehaviour it)
        {
            if (it.HasStateAuthority)
            {
                it.ReplicateToAll(false);
                it.ReplicateTo(it.Object.InputAuthority, true);
            }
        }
        
        public static bool IsRunning(this TickTimer timer, NetworkRunner runner) => 
            !timer.ExpiredOrNotRunning(runner);

        public static void SetIsSimulated(this NetworkObject it, bool isSimulated) => 
            it.Runner.SetIsSimulated(it, isSimulated);
        
        public static void SetIsSimulated(this NetworkBehaviour it, bool isSimulated) => 
            it.Runner.SetIsSimulated(it.Object, isSimulated);

    }
}