using Fusion;
using Game.Scripts.GameObjects.Core;
using UnityEngine;

namespace Game.Scripts.GameObjects.View.Combat
{
    public sealed class FireWeaponAnimComponent : NetworkBehaviour
    {
        private static readonly int Fire = Animator.StringToHash(nameof(Fire));

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
            _animator.SetTrigger(Fire);
        }
    }
}