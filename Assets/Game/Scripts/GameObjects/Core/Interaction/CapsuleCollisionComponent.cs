using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class CapsuleCollisionComponent : NetworkBehaviour
    {
        public interface IHandler
        {
            void OnCollision(NetworkObject other);
        }

        private static readonly Collider[] s_colliders = new Collider[32];

        [SerializeField]
        private CapsuleCollider _collider;

        [SerializeField]
        private LayerMask _layerMask;

        private IHandler _handler;

        public void SetHandler(IHandler handler)
        {
            _handler = handler;
        }

        public override void FixedUpdateNetwork()
        {
            // Реакция на столкновение (урон) - решение сервера, как и у WeaponComponent.
            if (!this.HasStateAuthority || _handler == null)
                return;

            _collider.GetPointsAndRadius(out Vector3 point0, out Vector3 point1, out float radius);

            int count = this.Runner.GetPhysicsScene().OverlapCapsule(
                point0,
                point1,
                radius,
                s_colliders,
                _layerMask,
                QueryTriggerInteraction.Collide
            );

            for (int i = 0; i < count; i++)
            {
                NetworkObject other = s_colliders[i].GetComponentInParent<NetworkObject>();

                // Собственные коллайдеры тоже попадают в капсулу - пропускаем себя.
                // IsValid: коллайдеры деспавненного в этом тике объекта ещё остаются в PhysX-сцене.
                if (other != null && other.IsValid && other != this.Object)
                    _handler.OnCollision(other);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                return;

            _collider.GetPointsAndRadius(out Vector3 p0, out Vector3 p1, out float radius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(p0, radius);
            Gizmos.DrawWireSphere(p1, radius);
            Gizmos.DrawLine(p0, p1);
        }
#endif
    }
}
