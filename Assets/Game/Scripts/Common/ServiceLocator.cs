using System;
using System.Collections.Generic;

namespace Game.Scripts.Common
{
    // Для объектов, создаваемых в рантайме (префабы), которые не могут сослаться на сцену.
    // Объекты сцены получают зависимости прямыми ссылками из инспектора.
    // Один сервис на тип — поэтому SpawnPointService (их два) сюда не регистрируется.
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> s_services = new();

        public static void Register(Type type, object service)
        {
            if (!s_services.TryAdd(type, service))
                throw new InvalidOperationException($"Сервис {type.Name} уже зарегистрирован.");
        }

        public static void Register<T>(T service) where T : class => Register(typeof(T), service);

        public static void Unregister(Type type, object service)
        {
            // Снимаем только свой экземпляр - чтобы не затереть сервис, зарегистрированный позже.
            if (s_services.TryGetValue(type, out object current) && ReferenceEquals(current, service))
                s_services.Remove(type);
        }

        public static void Unregister<T>(T service) where T : class => Unregister(typeof(T), service);

        public static bool TryGet<T>(out T service) where T : class
        {
            if (s_services.TryGetValue(typeof(T), out object value))
            {
                service = (T) value;
                return true;
            }

            service = null;
            return false;
        }

        public static T Get<T>() where T : class
        {
            if (!TryGet(out T service))
                throw new InvalidOperationException($"Сервис {typeof(T).Name} не зарегистрирован в ServiceLocator.");

            return service;
        }
    }
}
