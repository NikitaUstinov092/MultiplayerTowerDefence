using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
{
    public sealed class EnemyAi : NetworkBehaviour
    {
        [SerializeField]
        private MoveComponent _moveComponent;

        [SerializeField]
        private float _stopDistance = 1.5f;

        private Transform _portal;

        public override void Spawned()
        {
            PortalService portalService = FindObjectOfType<PortalService>();
            if (portalService != null)
                _portal = portalService.Portal;
        }

        public override void FixedUpdateNetwork()
        {
            if (_portal == null)
            {
                _moveComponent.Stop();
                return;
            }

            Vector3 offset = _portal.position - this.transform.position;
            offset.y = 0;

            if (offset.sqrMagnitude <= _stopDistance * _stopDistance)
            {
                _moveComponent.Stop();
                return;
            }

            _moveComponent.Move(offset.normalized);
        }
    }
}
