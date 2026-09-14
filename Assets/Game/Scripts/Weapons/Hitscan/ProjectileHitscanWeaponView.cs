using Fusion;
using Fusion.LagCompensation;
using UnityEngine;

namespace SampleGame
{
    public sealed class ProjectileHitscanWeaponView : NetworkBehaviour
    {
        [SerializeField]
        private HitscanWeapon _weapon;

        [SerializeField]
        private ParticleSystem _fireVfx;

        [SerializeField]
        private AudioSource _fireSfx;

        [SerializeField]
        private ProjectileHitscanVfx _projectilePrefab;

        [SerializeField]
        private ParticleSystem _bloodVfxPrefab;

        [SerializeField]
        private ParticleSystem _decalVfxPrefab;

        public override void Spawned()
        {
            if (this.enabled)
            {
                _weapon.OnFire += this.OnFire;
                _weapon.OnHitscanFire += this.OnHitscan;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (this.enabled)
            {
                _weapon.OnFire -= this.OnFire;
                _weapon.OnHitscanFire -= this.OnHitscan;
            }
        }

        private void OnFire()
        {
            // Эффект выстрела у дула
            _fireVfx.Play();

            // Звук выстрела у дула
            _fireSfx.Play();
        }

        private void OnHitscan(int _, HitscanEventData hitscanEvent)
        {
            Vector3 endPosition = hitscanEvent.endPosition;
            Quaternion vfxRotation = Quaternion.LookRotation(-hitscanEvent.Direction);

            // Рисуется болванка
            ProjectileHitscanVfx projectile =
                Instantiate(_projectilePrefab, hitscanEvent.startPosition, hitscanEvent.Rotation);
            projectile.Initialize(endPosition, _ =>
            {
                Destroy(projectile.gameObject);

                // Рисуется кровь на противнике или дырка в стене
                HitType hitType = hitscanEvent.hitType;
                if (hitType == HitType.PhysX)
                    SpawnVfx(_decalVfxPrefab, endPosition, vfxRotation);
                else if (hitType == HitType.Hitbox)
                    SpawnVfx(_bloodVfxPrefab, endPosition, vfxRotation);
            });
        }

        private static void SpawnVfx(ParticleSystem vfxPrefab, Vector3 position, Quaternion rotation)
        {
            ParticleSystem vfx = Instantiate(vfxPrefab);
            vfx.transform.SetLocalPositionAndRotation(position, rotation);
            vfx.Play(withChildren: true);
            Destroy(vfx.gameObject, vfx.main.duration);
        }
    }
}