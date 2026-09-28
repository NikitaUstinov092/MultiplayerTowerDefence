using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerPurchaseController : NetworkBehaviour
    {
        private TeamWallet _teamWallet;
        
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            ServiceLocator.TryGet(out _teamWallet);
        }

        public override void FixedUpdateNetwork()
        {
            if (!this.HasInputAuthority)
                return;

            if (!this.GetInput(out PlayerInputData inputData))
                return;

            if (_teamWallet != null)
            {
                TryBuy(inputData.buttons, PlayerInputButtons.BuyMine, PlayerKeys.Mine);
                TryBuy(inputData.buttons, PlayerInputButtons.BuyArcher, PlayerKeys.Archer);
            }

            _previousButtons = inputData.buttons;
        }

        private void TryBuy(NetworkButtons buttons, PlayerInputButtons button, PlayerKeys key)
        {
            if (buttons.WasPressed(_previousButtons, button))
                _teamWallet.RpcTryBuy(key);
        }
    }
}
