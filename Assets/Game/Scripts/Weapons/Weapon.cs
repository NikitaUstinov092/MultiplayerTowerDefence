using Fusion;

namespace SampleGame
{
    public abstract class Weapon : NetworkBehaviour
    {
        public abstract bool CanFire();

        public abstract void Fire();
    }
}