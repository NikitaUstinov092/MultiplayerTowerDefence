using System;
using Fusion;
using Game;
using UnityEngine;

namespace SampleGame
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
            if (this.HasStateAuthority)
                this.Balance = _startingBalance;
        }

        public void AddCoins(int amount)
        {
            if (!this.HasStateAuthority || amount <= 0)
                return;

            this.Balance += amount;
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, TickAligned = false, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcTryBuy(PlayerKeys id, RpcInfo info = default)
        {
            if (!this.HasStateAuthority || _catalog == null)
                return;

            if (!_catalog.TryGetConfig(id, out PurchasableConfig config))
                return;

            if (this.Balance < config.Price)
                return;

            Vector3 spawnPosition = this.transform.position;
            if (this.Runner.TryGetPlayerObject(info.Source, out NetworkObject buyer))
            {
                spawnPosition = buyer.TryGetBehaviour(out PlayerInputController inputController) && inputController.Character != null
                    ? inputController.Character.transform.position
                    : buyer.transform.position;
            }
            
            this.Runner.Spawn(config.Prefab, spawnPosition, Quaternion.identity);
            this.Balance -= config.Price;
        }

        private void InvokeBalanceChanged(NetworkBehaviourBuffer previousSnapshot)
        {
            this.OnBalanceChanged?.Invoke(this.Balance);
        }
    }
}
