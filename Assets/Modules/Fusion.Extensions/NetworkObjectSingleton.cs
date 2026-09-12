namespace Fusion
{
    public abstract class NetworkObjectSingleton<T> : NetworkObject where T : NetworkObjectSingleton<T>
    {
        public static T Instance { get; private set; }

        protected override void Awake()
        {
            Instance = (T) this;
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (Instance == this)
                Instance = null;
        }
    }
}