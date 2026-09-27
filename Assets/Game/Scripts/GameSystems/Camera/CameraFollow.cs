using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace SampleGame
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private NetworkRunner _runner;

        [SerializeField]
        private float _smoothTime = 0.25f;

        private Transform _target;
        private Vector3 _velocity;

        // LateUpdate идёт после Render Fusion - позиция персонажа уже интерполирована, камера не дрожит.
        private void LateUpdate()
        {
            if (_target == null && !this.TryFindLocalPlayer(out _target))
                return;

            this.transform.position = Vector3.SmoothDamp(
                this.transform.position,
                _target.position,
                ref _velocity,
                _smoothTime
            );
        }

        // Ищем своего персонажа по InputAuthority, а не через GetPlayerObject:
        // SetPlayerObject вызывается только на сервере, клиент о нём может не знать.
        private bool TryFindLocalPlayer(out Transform target)
        {
            target = null;
            if (_runner == null || !_runner.IsRunning)
                return false;

            List<NetworkObject> buffer = ListPool<NetworkObject>.Get();
            _runner.GetAllNetworkObjects(buffer);

            for (int i = 0, count = buffer.Count; i < count; i++)
            {
                NetworkObject obj = buffer[i];
                if (obj.HasInputAuthority && obj.TryGetBehaviour(out Player _))
                {
                    target = obj.transform;
                    break;
                }
            }

            ListPool<NetworkObject>.Release(buffer);

            if (target == null)
                return false;

            // Первый захват - сразу на игрока, без перелёта из начала координат.
            this.transform.position = target.position;
            _velocity = Vector3.zero;
            return true;
        }
    }
}
