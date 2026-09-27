using UnityEngine;

namespace SampleGame
{
    // Регистрация раньше всех скриптов - чтобы сервисы были доступны уже в их Awake/Spawned.
    [DefaultExecutionOrder(-1000)]
    public sealed class ServiceLocatorInstaller : MonoBehaviour
    {
        [SerializeField]
        private Component[] _services;

        private void Awake()
        {
            foreach (Component service in _services)
            {
                if (service != null)
                    ServiceLocator.Register(service.GetType(), service);
            }
        }

        private void OnDestroy()
        {
            foreach (Component service in _services)
            {
                if (service != null)
                    ServiceLocator.Unregister(service.GetType(), service);
            }
        }
    }
}
