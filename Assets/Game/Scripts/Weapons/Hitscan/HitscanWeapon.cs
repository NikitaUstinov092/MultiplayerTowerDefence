using System;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace SampleGame
{
    public sealed class HitscanWeapon : Weapon
    {
        public event Action OnFire;
        public event Action<int, HitscanEventData> OnHitscanFire; 
        
        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private float _fireDistance = 10;

        [SerializeField]
        private int _damage = 1;

        [SerializeField]
        private float _cooldown = 1;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private int _hitscansPerShoot = 5;

        [Tooltip("Dispersion in angles")]
        [SerializeField]
        private int _dispersion = 10; // 4-6

        [Networked]
        private TickTimer _cooldownTimestamp { get; set; }

        [Networked, Capacity(8)]
        private NetworkArray<HitscanEventData> _hitscanEvents { get; }

        [Networked]
        private int _fireCount { get; set; } // Выстрелы

        [Networked]
        private int _hitscanCount { get; set; } // Снаряды

        private int _localFireCount; // Выстрелы

        private int _localHitscanCount; // Снаряды

        public override void Spawned()
        {
            _localFireCount = _fireCount;
            _localHitscanCount = _hitscanCount;
        }

        public override void Render()
        {
            while (_localHitscanCount < _hitscanCount)
            {
                int index = _localHitscanCount % _hitscanEvents.Length;
                HitscanEventData hitscanEvent = _hitscanEvents[index];
                this.OnHitscanFire?.Invoke(index, hitscanEvent);
                _localHitscanCount++;
            }
            
            while (_localFireCount < _fireCount)
            {
                this.OnFire?.Invoke();
                _localFireCount = _fireCount;
            }
        }

        public override bool CanFire()
        {
            return _cooldownTimestamp.ExpiredOrNotRunning(this.Runner);
        }

        // FUN (Client-Side / Server) Red //16
        public override void Fire()
        {
            HitboxManager hitboxManager = this.Runner.LagCompensation;
            Vector3 startPosition = _firePoint.position;

            int seed = this.Runner.Tick.Raw * unchecked((int) this.Object.Id.Raw);
            Random.InitState(seed);
            
            for (int i = 0; i < _hitscansPerShoot; i++)
            {
                Vector3 endPosition;
                HitType hitType = HitType.None;
                Vector3 direction = this.GetRandomDirection();

                bool wasHit = hitboxManager.Raycast(
                    startPosition,
                    direction,
                    _fireDistance,
                    this.Object.InputAuthority,
                    out LagCompensatedHit hit,
                    _layerMask,
                    HitOptions.SubtickAccuracy | HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority,
                    preProcessRoots: PreProcessHitboxes
                );

                if (wasHit)
                {
                    hitType = hit.Type;

                    if (hitType == HitType.PhysX)
                        Debug.Log($"WAS HIT INTO COLLIDER {hit.Collider.name}", hit.Collider);
                    else if (hitType == HitType.Hitbox)
                        Debug.Log($"WAS HIT INTO HITBOX {hit.Hitbox.name}", hit.Hitbox);

                    this.DealDamage(hit, this.Object.InputAuthority);

                    endPosition = hit.Point;
                }
                else
                {
                    endPosition = startPosition + direction * _fireDistance;
                }

                _hitscanEvents.Set(_hitscanCount % _hitscanEvents.Length, new HitscanEventData
                {
                    startPosition = startPosition,
                    endPosition = endPosition,
                    hitType = hitType
                });

                _hitscanCount++;
            }
            
            _cooldownTimestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
            _fireCount++;
        }

        private Vector3 GetRandomDirection()
        {
            if (_dispersion <= 0f)
                return _firePoint.forward;

            Vector2 dispersionDirection = Random.insideUnitCircle * _dispersion;
            Quaternion dispersionRotation = Quaternion.Euler(dispersionDirection.y, dispersionDirection.x, 0f);
            return _firePoint.rotation * dispersionRotation * Vector3.forward;
        }

        private static void PreProcessHitboxes(Query query,
            HashSet<HitboxRoot> rootCandidates, // Exclude 
            HashSet<int> processedColliderIndices //Include
        )
        {
            rootCandidates.RemoveWhere(ShouldRemoveHitboxRoot);
        }

        private static bool ShouldRemoveHitboxRoot(HitboxRoot hitboxRoot)
        {
            return !hitboxRoot.TryGetBehaviour(out HealthComponent health) || !health.IsAlive;
        }

        private void DealDamage(LagCompensatedHit hit, PlayerRef player)
        {
            CharacterHitbox hitbox = hit.Hitbox as CharacterHitbox;
            if (hitbox == null)
                return;

            int fullDamage = Mathf.RoundToInt(_damage * hitbox.DamageMultiplier);
            HealthComponent health = hitbox.GetComponentInParent<HealthComponent>();
            Debug.Log($"DEAL DAMAGE {fullDamage} TO TARGET {health.name}", health);
            health.TakeDamage(fullDamage, player);

            if (hitbox.Detachable)
            {
                // hitbox.Root.SetHitboxActive(hitbox, false); // 0 -> 1
                hitbox.HitboxActive = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_firePoint.position, _firePoint.position + _firePoint.forward * _fireDistance);
            }
        }
    }
}