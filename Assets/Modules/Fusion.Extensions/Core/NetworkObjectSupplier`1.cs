namespace Fusion
{
    public abstract class NetworkObjectSupplier<T> : NetworkObjectSupplier where T : NetworkBehaviour
    {
        public bool Supply(PlayerRef inputAuthority, out T behaviour)
        {
            if (this.Supply(inputAuthority, out NetworkObject instance))
            {
                behaviour = instance.GetBehaviour<T>();
                return true;
            }

            behaviour = null;
            return false;
        }
    }
}