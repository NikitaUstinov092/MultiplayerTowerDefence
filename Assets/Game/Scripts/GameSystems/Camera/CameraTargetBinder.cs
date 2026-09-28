using Fusion;

namespace SampleGame
{
    public sealed class CameraTargetBinder : NetworkBehaviour
    {
        private CameraFollow _camera;

        public override void Spawned()
        {
            if (this.HasInputAuthority && ServiceLocator.TryGet(out _camera))
                _camera.SetTarget(this.transform);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (_camera != null)
                _camera.ClearTarget(this.transform);

            _camera = null;
        }
    }
}
