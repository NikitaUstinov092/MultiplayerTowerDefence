using System;
using Fusion;
using Game.Scripts.GameSystems.Player.Input;
using Game.Scripts.Input;
using UnityEngine;

namespace Game.Scripts.GameSystems.Economy
{
    public sealed class TeamWallet : NetworkBehaviour
    {
        [SerializeField] private PurchasableCatalog _catalog;
        [SerializeField] private int _startingBalance;

        [Networked, OnChangedRender(nameof(InvokeBalanceChanged))]
        public int Balance { get; private set; }

        public event Action<int> OnBalanceChanged;

        public override void Spawned()
        {
            if (HasStateAuthority)
                Balance = _startingBalance;
        }

        public void AddCoins(int amount)
        {
            if (!HasStateAuthority || amount <= 0)
                return;

            Balance += amount;
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, TickAligned = false, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcTryBuy(PlayerKeys id, RpcInfo info = default)
        {
            if (!HasStateAuthority || _catalog == null)
                return;

            if (!_catalog.TryGetConfig(id, out PurchasableConfig config))
                return;

            if (Balance < config.Price)
                return;

            Vector3 spawnPosition = transform.position;
            if (Runner.TryGetPlayerObject(info.Source, out NetworkObject buyer))
            {
                spawnPosition = buyer.TryGetBehaviour(out PlayerInputController inputController) && inputController.Character != null
                    ? inputController.Character.transform.position
                    : buyer.transform.position;
            }
            
            Runner.Spawn(config.Prefab, spawnPosition, Quaternion.identity);
            Balance -= config.Price;
        }

        private void InvokeBalanceChanged(NetworkBehaviourBuffer previousSnapshot)
        {
            OnBalanceChanged?.Invoke(Balance);
        }
    }
}
