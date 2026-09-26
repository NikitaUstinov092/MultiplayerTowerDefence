using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class ExplosionComponent : NetworkBehaviour
    {
        public interface IHandler
        {
            void OnHit(NetworkObject target);
        }

        public event Action OnExploded;

        private static readonly Collider[] s_colliders = new Collider[64];
        private static readonly HashSet<NetworkObject> s_hitObjects = new();

        [SerializeField]
        private float _radius = 3f;

        [SerializeField]
        private LayerMask _layerMask;

        [Networked, OnChangedRender(nameof(InvokeExploded))] // Server -> Client
        public NetworkBool IsExploded { get; private set; }

        private IHandler _handler;

        public void SetHandler(IHandler handler)
        {
            _handler = handler;
        }

        // Вызывать только на StateAuthority.
        public void Explode()
        {
            if (!this.HasStateAuthority || this.IsExploded)
                return;

            int count = this.Runner.GetPhysicsScene().OverlapSphere(
                this.transform.position,
                _radius,
                s_colliders,
                _layerMask,
                QueryTriggerInteraction.Collide
            );

            // У одного объекта может быть несколько коллайдеров (тело + сенсор) - бьём каждый объект один раз.
            s_hitObjects.Clear();
            for (int i = 0; i < count; i++)
            {
                NetworkObject target = s_colliders[i].GetComponentInParent<NetworkObject>();

                // IsValid: коллайдеры деспавненного в этом тике объекта ещё остаются в PhysX-сцене.
                if (target == null || !target.IsValid || target == this.Object || !s_hitObjects.Add(target))
                    continue;

                _handler?.OnHit(target);
            }

            this.IsExploded = true;
        }

        private void InvokeExploded()
        {
            if (this.IsExploded)
                this.OnExploded?.Invoke();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, _radius);
        }
#endif
    }
}
