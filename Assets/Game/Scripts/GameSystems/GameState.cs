using System;
using Fusion;

namespace Game.Scripts.GameSystems
{
    public sealed class GameState : NetworkBehaviour
    {
        [Networked, OnChangedRender(nameof(InvokeGameOver))]
        public NetworkBool IsGameOver { get; private set; }

        public event Action OnGameOver;

        // OnChangedRender не срабатывает на начальное значение - клиент, подключившийся
        // после поражения, иначе не узнает о нём.
        public override void Spawned()
        {
            InvokeGameOver();
        }

        public void SetGameOver()
        {
            if (!HasStateAuthority || IsGameOver)
                return;

            IsGameOver = true;
        }

        private void InvokeGameOver()
        {
            if (IsGameOver)
                OnGameOver?.Invoke();
        }
    }
}
