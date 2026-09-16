using System;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    public abstract class GrenadeBase : NetworkBehaviour
    {
        private static readonly Collider[] s_colliders = new Collider[16];

        public event Action<PlayerRef> OnOwnerChanged;

        [SerializeField]
        protected Rigidbody _rigidbody;

        [SerializeField]
        private int _explodeRadius = 3;

        [SerializeField]
        private int _explodeDamage = 10;

        [SerializeField]
        protected float _cooldown = 4;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private float _explosionForce = 15f;

        [SerializeField]
        private float _upwardsModifier = 0.5f;

        [Networked]
        public TickTimer Timestamp { get; protected set; }

        [Networked]
        public Tick ThrowTick { get; protected set; }

        [Networked, OnChangedRender(nameof(InvokeOwnerChanged))]
        public PlayerRef Owner { get; set; } //int

        public abstract void Teleport(Vector3 position, Quaternion rotation);

        public void Throw(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = Vector3.zero;
            this.Timestamp = TickTimer.CreateFromSeconds(this.Runner, _cooldown);
            this.ThrowTick = this.Runner.Tick;
        }

        public override void FixedUpdateNetwork()
        {
            if (this.Timestamp.Expired(this.Runner))
                this.Explode();
        }

        private void Explode()
        {
            Vector3 explosionPosition = _rigidbody.position;
            int count = this.Runner.GetPhysicsScene().OverlapSphere(
                explosionPosition, _explodeRadius,
                s_colliders,
                _layerMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < count; i++)
            {
                Collider hit = s_colliders[i];
                NetworkObject other = hit.GetComponentInParent<NetworkObject>();
                if (other == null || other == this.Object)
                    continue;

                if (other.TryGetBehaviour(out HealthComponent health)
                    && health.IsAlive)
                    health.TakeDamage(_explodeDamage, this.Owner);

                if (other.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(
                        _explosionForce,
                        explosionPosition,
                        _explodeRadius,
                        _upwardsModifier,
                        ForceMode.Impulse
                    );
            }

            if (this.HasStateAuthority)
                this.Runner.Despawn(this.Object);
        }

        private void InvokeOwnerChanged() =>
            this.OnOwnerChanged?.Invoke(this.Owner);
    }
}