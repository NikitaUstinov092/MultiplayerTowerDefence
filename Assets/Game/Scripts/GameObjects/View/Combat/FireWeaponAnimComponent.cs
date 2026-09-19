using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class FireWeaponAnimComponent : NetworkBehaviour
    {
        private static readonly int Fire = Animator.StringToHash(nameof(Fire));

        //TODO в Animator Controller нет параметра MeleeFire, добавить и раскомментировать
        // private static readonly int MeleeFire = Animator.StringToHash(nameof(MeleeFire));

        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private Animator _animator;

        public override void Spawned()
        {
            if (this.enabled)
                _weaponComponent.OnFireStarted += this.OnFire;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (this.enabled)
                _weaponComponent.OnFireStarted -= this.OnFire;
        }

        private void OnFire()
        {
            // Kiss
            Weapon weapon = _weaponComponent.Current;
            if (weapon is ProjectileWeapon)
                _animator.SetTrigger(Fire);
            //TODO в Animator Controller нет параметра MeleeFire, добавить и раскомментировать
            // else if (weapon is MeleeWeapon)
            //     _animator.SetTrigger(MeleeFire);
        }
    }
}