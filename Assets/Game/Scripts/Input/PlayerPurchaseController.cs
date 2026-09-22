using Fusion;
using Game;

namespace SampleGame
{
    public sealed class PlayerPurchaseController : NetworkBehaviour
    {
        private TeamWallet _teamWallet;

        [Networked]
        private NetworkButtons _previousButtons { get; set; }

        public override void Spawned()
        {
            WalletService walletService = FindObjectOfType<WalletService>();
            if (walletService != null)
                _teamWallet = walletService.TeamWallet;
        }

        public override void FixedUpdateNetwork()
        {
            if (!this.GetInput(out PlayerInputData inputData))
                return;

            if (_teamWallet != null)
            {
                if (inputData.buttons.WasPressed(_previousButtons, PlayerInputButtons.BuyMine))
                    _teamWallet.RpcTryBuy(PlayerKeys.Mine);

                if (inputData.buttons.WasPressed(_previousButtons, PlayerInputButtons.BuyArcher))
                    _teamWallet.RpcTryBuy(PlayerKeys.Archer);
            }

            _previousButtons = inputData.buttons;
        }
    }
}
