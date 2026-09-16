using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class ProjectileWeaponView : NetworkBehaviour
    {
        [SerializeField]
        private ProjectileWeapon _weapon;

        [SerializeField]
        private AudioSource _audioSource;

        public override void Spawned()
        {
            _weapon.OnFire += this.OnFire;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _weapon.OnFire -= this.OnFire;
        }

        private void OnFire()
        {
            _audioSource.Play();
        }
    }
}
