using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
    [DisallowMultipleComponent]
    public class NetworkSceneManagerAdvanced : NetworkSceneManagerDefault
    {
        protected override IEnumerator OnSceneLoaded(
            SceneRef sceneRef,
            Scene scene,
            NetworkLoadSceneParameters sceneParams
        )
        {
            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams); // NetworkObject -> Scene
            this.AddSimulationBehaviours(scene);
        }

        private void AddSimulationBehaviours(Scene scene)
        {
            SimulationBehaviour[] simulationBehaviours = scene.GetComponentsInHierarchyOrder<SimulationBehaviour>();
            foreach (SimulationBehaviour simulationBehaviour in simulationBehaviours)
                if (simulationBehaviour is not NetworkBehaviour)
                    this.Runner.AddGlobal(simulationBehaviour);
        }

        protected override void CleanupSceneBeforeUnload(Scene scene, SceneRef sceneRef)
        {
            this.RemoveSimulationBehaviours(scene);
            base.CleanupSceneBeforeUnload(scene, sceneRef);
        }

        private void RemoveSimulationBehaviours(Scene scene)
        {
            NetworkRunner runner = this.Runner;
            if (!runner || !runner.IsRunning)
                return;

            SimulationBehaviour[] simulationBehaviours = scene.GetComponentsInHierarchyOrder<SimulationBehaviour>();
            foreach (SimulationBehaviour simulationBehaviour in simulationBehaviours)
                if (simulationBehaviour && simulationBehaviour is not NetworkBehaviour &&
                    simulationBehaviour.Runner == runner && simulationBehaviour.Object == null)
                    runner.RemoveGlobal(simulationBehaviour);
        }
    }
}