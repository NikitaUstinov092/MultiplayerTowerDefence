using Fusion;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class LifetimeComponent : NetworkBehaviour
    {
        public interface IHandler
        {
            void OnExpired();
        }

        [SerializeField]
        private float _lifetime = 15f;

        [Networked]
        private TickTimer _lifetimeTimestamp { get; set; }

        private IHandler _handler;

        public void SetHandler(IHandler handler)
        {
            _handler = handler;
        }

        public override void Spawned()
        {
            if (this.HasStateAuthority)
                _lifetimeTimestamp = TickTimer.CreateFromSeconds(this.Runner, _lifetime);
        }

        public override void FixedUpdateNetwork()
        {
            // Истечение срока жизни - решение сервера, как и деспавн.
            if (!this.HasStateAuthority || !_lifetimeTimestamp.Expired(this.Runner))
                return;

            // Сброс таймера - чтобы обработчик сработал ровно один раз.
            _lifetimeTimestamp = default;
            _handler?.OnExpired();
        }
    }
}
