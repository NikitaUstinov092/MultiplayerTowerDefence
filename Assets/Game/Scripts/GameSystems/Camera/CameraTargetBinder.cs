using Fusion;
using Game.Scripts.Common;

namespace Game.Scripts.GameSystems.Camera
{
    public sealed class CameraTargetBinder : NetworkBehaviour
    {
        private CameraFollow _camera;

        public override void Spawned()
        {
            if (HasInputAuthority && ServiceLocator.TryGet(out _camera))
                _camera.SetTarget(transform);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (_camera != null)
                _camera.ClearTarget(transform);

            _camera = null;
        }
    }
}
