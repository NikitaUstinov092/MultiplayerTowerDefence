using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
{
    public sealed class PlayerPurchaseController : NetworkBehaviour
    {
        private TeamWallet _teamWallet;

        // Не [Networked]: чисто локальная отметка для детекции фронта нажатия на этой же машине,
        // не часть реплицируемого состояния - ей незачем восстанавливаться (rollback) при ресимуляции.
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            WalletService walletService = FindObjectOfType<WalletService>();
            if (walletService != null)
                _teamWallet = walletService.TeamWallet;
        }

        public override void FixedUpdateNetwork()
        {
            // RpcTryBuy - не идемпотентный побочный эффект (тратит деньги и спавнит объект),
            // поэтому логика покупки должна выполняться ровно в одном симуляционном контексте.
            // Без этой проверки в Host-режиме хост тоже симулирует объект клиента (у хоста есть
            // StateAuthority над ним) и вызывает RPC второй раз на то же самое нажатие.
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
