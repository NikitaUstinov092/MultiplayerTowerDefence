using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
    [DisallowMultipleComponent]
    public sealed class NetworkSceneManagerZenject : NetworkSceneManagerAdvanced
    {
        protected override IEnumerator OnSceneLoaded(
            SceneRef sceneRef,
            Scene scene,
            NetworkLoadSceneParameters sceneParams
        )
        {
            this.InitializeSceneContext(scene);
            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
        }

        protected override void CleanupSceneBeforeUnload(Scene scene, SceneRef sceneRef)
        {
            this.UnsubscribeFromObjectAcquired(scene);
            base.CleanupSceneBeforeUnload(scene, sceneRef);
        }

        private void InitializeSceneContext(Scene scene)
        {
            NetworkRunnerContext runnerContext = this.Runner.GetComponent<NetworkRunnerContext>();
            if (!runnerContext)
            {
                Debug.LogWarning($"{nameof(NetworkRunnerContext)} is not added to {nameof(NetworkRunner)}.");
                return;
            }

            NetworkSceneContext sceneContext = scene.GetComponentInHierarchyOrder<NetworkSceneContext>();
            if (!sceneContext)
            {
                Debug.LogWarning($"{nameof(NetworkSceneContext)} is not added to the loaded scene.");
                return;
            }

            sceneContext.Run(runnerContext);
            this.SubscribeToObjectAcquired(scene, sceneContext);
        }

        private void SubscribeToObjectAcquired(Scene scene, NetworkSceneContext sceneContext)
        {
            if (scene == this.MainRunnerScene)
                this.Runner.ObjectAcquired += sceneContext.ProcessObjectAcquired;
        }

        private void UnsubscribeFromObjectAcquired(Scene scene)
        {
            if (!this.Runner || !this.Runner.IsRunning || scene != this.MainRunnerScene)
                return;

            NetworkSceneContext sceneContext = scene.GetComponentInHierarchyOrder<NetworkSceneContext>();
            if (sceneContext)
                this.Runner.ObjectAcquired -= sceneContext.ProcessObjectAcquired;
        }
    }
}