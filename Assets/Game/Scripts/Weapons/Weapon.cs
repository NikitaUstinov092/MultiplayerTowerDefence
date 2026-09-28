using Fusion;

namespace Game.Scripts.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        public abstract bool CanFire();

        public abstract void Fire();
    }
}