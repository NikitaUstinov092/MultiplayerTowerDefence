using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleGame
{
    public sealed class FusionBootstrap : MonoBehaviour
    {
        [SerializeField]
        private NetworkRunner _runner;
        
        private async void Start()
        {
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));
            
            
            await _runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "SampleSession",
                PlayerCount = 2,
                Scene = sceneInfo,
                SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }
    }
}