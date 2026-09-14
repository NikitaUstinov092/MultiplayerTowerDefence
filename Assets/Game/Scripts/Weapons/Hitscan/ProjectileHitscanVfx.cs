using System;
using UnityEngine;

namespace SampleGame
{
    public sealed class ProjectileHitscanVfx : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 30f;

        [SerializeField]
        private TrailRenderer _trailRenderer;

        [SerializeField]
        private ParticleSystem _explosionVfx;

        private Vector3 _destination;
        private Action<ProjectileHitscanVfx> _onDestroy;
        private bool _initialized;

        public void Initialize(
            Vector3 destination,
            Action<ProjectileHitscanVfx> onDestroy)
        {
            _destination = destination;
            _onDestroy = onDestroy;
            _initialized = true;

            // Если нужно, чтобы пуля всегда смотрела в сторону цели
            Vector3 direction = _destination - transform.position;

            if (direction.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Update()
        {
            if (!_initialized)
                return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                _destination,
                _speed * Time.deltaTime);

            if (transform.position == _destination) 
                Complete();
        }

        private void Complete()
        {
            _initialized = false;

            // Гарантируем точную конечную позицию
            transform.position = _destination;

            // Сначала VFX, пока projectile ещё находится в нужной точке
            SpawnExplosion();

            if (_trailRenderer != null)
            {
                _trailRenderer.transform.SetParent(null, true);
                Destroy(_trailRenderer.gameObject, 1.5f);
            }

            _onDestroy?.Invoke(this);
        }

        private void SpawnExplosion()
        {
            if (_explosionVfx == null)
                return;

            ParticleSystem explosion = Instantiate(
                _explosionVfx,
                _destination,
                transform.rotation
            );

            explosion.Play(true);

            float lifetime = explosion.main.duration;
            if (explosion.main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                lifetime += explosion.main.startLifetime.constant;

            Destroy(explosion.gameObject, lifetime);
        }
    }
}