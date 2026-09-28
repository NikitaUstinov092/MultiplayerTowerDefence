using UnityEngine;

namespace Game.Scripts.GameSystems.Camera
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private float _smoothTime = 0.25f;

        private Transform _target;
        private Vector3 _velocity;

        public void SetTarget(Transform target)
        {
            _target = target;
            
            this.transform.position = target.position;
            _velocity = Vector3.zero;
        }

        public void ClearTarget(Transform target)
        {
            if (_target == target)
                _target = null;
        }
        
        private void LateUpdate()
        {
            if (_target == null)
                return;

            this.transform.position = Vector3.SmoothDamp(
                this.transform.position,
                _target.position,
                ref _velocity,
                _smoothTime
            );
        }
    }
}
