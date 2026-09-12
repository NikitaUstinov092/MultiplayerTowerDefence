using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.Internal;

namespace Fusion
{
    [RequireComponent(typeof(NetworkRunner))]
    [RequireComponent(typeof(NetworkEvents))]
    [RequireComponent(typeof(NetworkSceneManagerZenject))]
    public class NetworkRunnerContext : RunnableContext
    {
        public override DiContainer Container => _container;

        private DiContainer _container;
        private bool _hasInstalled;

        private void Awake()
        {
            this.Initialize();
        }

        protected override void RunInternal()
        {
            if (_hasInstalled)
                return;

            _hasInstalled = true;

            ProjectContext projectContext = ProjectContext.Instance;
            projectContext.EnsureIsInitialized();

            ModestTree.Assert.IsNull(_container);

            _container = projectContext.Container.CreateSubContainer();
            _container.DefaultParent = this.transform;

            this.QueueInjectables();

            _container.IsInstalling = true;

            try
            {
                _container.Bind<NetworkRunner>().FromComponentOn(this.gameObject).AsSingle();
                _container.Bind<NetworkEvents>().FromComponentOn(this.gameObject).AsSingle();

                this.InstallInstallers();
            }
            finally
            {
                _container.IsInstalling = false;
            }

            _container.ResolveRoots();
        }

        private void QueueInjectables()
        {
            List<MonoBehaviour> monoBehaviours = UnityEngine.Pool.ListPool<MonoBehaviour>.Get();
            try
            {
                this.GetInjectableMonoBehaviours(monoBehaviours);
                foreach (MonoBehaviour instance in monoBehaviours)
                    _container.QueueForInject(instance);
            }
            finally
            {
                UnityEngine.Pool.ListPool<MonoBehaviour>.Release(monoBehaviours);
            }
        }

        protected override void GetInjectableMonoBehaviours(List<MonoBehaviour> monoBehaviours)
        {
            foreach (MonoBehaviour monoBehaviour in GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null || monoBehaviour == this)
                    continue;

                if (!ZenUtilInternal.IsInjectableMonoBehaviourType(monoBehaviour.GetType()))
                    continue;

                monoBehaviours.Add(monoBehaviour);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                ZenUtilInternal.GetInjectableMonoBehavioursUnderGameObject(child.gameObject, monoBehaviours);
            }
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            yield return gameObject;
        }
    }
}