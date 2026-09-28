using UnityEngine;

namespace SampleGame
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

            // Первый захват - сразу на игрока, без перелёта из начала координат.
            this.transform.position = target.position;
            _velocity = Vector3.zero;
        }

        public void ClearTarget(Transform target)
        {
            // Снимаем только свою цель - чтобы деспавн чужого объекта не сбросил текущую.
            if (_target == target)
                _target = null;
        }

        // LateUpdate идёт после Render Fusion - позиция персонажа уже интерполирована, камера не дрожит.
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
