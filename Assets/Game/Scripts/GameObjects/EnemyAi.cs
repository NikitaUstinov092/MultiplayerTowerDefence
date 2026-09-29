using Fusion;
using Game.Scripts.Common;
using Game.Scripts.GameObjects.Core;
using Game.Scripts.GameSystems;
using UnityEngine;

namespace Game.Scripts.GameObjects
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
            if (ServiceLocator.TryGet(out PortalPointService portalService))
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
