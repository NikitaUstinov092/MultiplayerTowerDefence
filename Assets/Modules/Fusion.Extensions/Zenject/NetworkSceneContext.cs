using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.Internal;

namespace Fusion
{
    [DisallowMultipleComponent]
    public class NetworkSceneContext : Context
    {
        private bool _hasInstalled;
        private bool _hasResolved;
        private DiContainer _container;

        public override DiContainer Container => _container;

        internal void Run(NetworkRunnerContext runnerContext)
        {
            ModestTree.Assert.IsNotNull(runnerContext);
            ModestTree.Assert.That(runnerContext.Initialized);

            ModestTree.Assert.That(!_hasInstalled);
            ModestTree.Assert.That(!_hasResolved);
            this.Install(runnerContext);
            this.Resolve();
        }

        private void Install(NetworkRunnerContext runnerContext)
        {
            ModestTree.Assert.That(!_hasInstalled);
            ModestTree.Assert.IsNull(_container);

            _hasInstalled = true;

            _container = runnerContext.Container.CreateSubContainer();
            _container.DefaultParent = null;
            _container.IsInstalling = true;

            try
            {
                this.InstallInstallers();
            }
            finally
            {
                _container.IsInstalling = false;
            }
        }

        private void Resolve()
        {
            ModestTree.Assert.That(_hasInstalled);
            ModestTree.Assert.That(!_hasResolved);


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

            _hasResolved = true;
            _container.ResolveRoots();
        }

        internal void ProcessObjectAcquired(NetworkRunner runner, NetworkObject networkObject)
        {
            if (networkObject)
                _container.InjectGameObject(networkObject.gameObject);
        }

        protected override void GetInjectableMonoBehaviours(List<MonoBehaviour> monoBehaviours)
        {
            MonoBehaviour[] behaviours = this.gameObject.scene.GetComponentsInHierarchyOrder<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
                if (behaviour && behaviour is not NetworkBehaviour && behaviour is not NetworkObject)
                    monoBehaviours.Add(behaviour);
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return ZenUtilInternal.GetRootGameObjects(this.gameObject.scene);
        }
    }
}