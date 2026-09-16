using System.Collections;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class FireMeleeSfxComponent : NetworkBehaviour
    {
        [SerializeField]
        private WeaponComponent _weaponComponent;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _meleeSfx;

        [SerializeField, Min(0f)]
        private float _delay = 0.2f;

        private Coroutine _coroutine;

        public override void Spawned()
        {
            _weaponComponent.OnFireStarted += OnFire;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _weaponComponent.OnFireStarted -= OnFire;
            this.CancelPlay();
        }

        private void OnFire()
        {
            this.CancelPlay();

            if (_weaponComponent.Current is MeleeWeapon) 
                _coroutine = this.StartCoroutine(this.PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            if (_delay > 0f)
                yield return new WaitForSeconds(_delay);

            _coroutine = null;

            _audioSource.PlayOneShot(_meleeSfx);
        }

        private void CancelPlay()
        {
            if (_coroutine == null)
                return;

            this.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}