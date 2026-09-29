using Fusion;
using Game.Scripts.Common;
using Game.Scripts.GameSystems.Economy;
using Game.Scripts.GameSystems.Player.Input;

namespace Game.Scripts.Input
{
    public sealed class PlayerPurchaseInputController : NetworkBehaviour
    {
        private TeamWallet _teamWallet;
        
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            ServiceLocator.TryGet(out _teamWallet);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority)
                return;

            if (!GetInput(out PlayerInputData inputData))
                return;

            if (_teamWallet != null)
            {
                TryBuy(inputData.Buttons, PlayerInputButtons.BuyMine, PlayerKeys.Mine);
                TryBuy(inputData.Buttons, PlayerInputButtons.BuyArcher, PlayerKeys.Archer);
            }

            _previousButtons = inputData.Buttons;
        }

        private void TryBuy(NetworkButtons buttons, PlayerInputButtons button, PlayerKeys key)
        {
            if (buttons.WasPressed(_previousButtons, button))
                _teamWallet.RpcTryBuy(key);
        }
    }
}
