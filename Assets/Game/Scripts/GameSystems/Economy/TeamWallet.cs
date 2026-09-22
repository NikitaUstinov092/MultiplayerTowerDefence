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
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, TickAligned = false, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcTryBuy(PlayerKeys id, RpcInfo info = default)
        {
            // ВРЕМЕННЫЙ ДИАГНОСТИЧЕСКИЙ ЛОГ - убрать после отладки двойного спавна.
            Debug.Log($"[RpcTryBuy] id={id} source={info.Source} tick={this.Runner.Tick}");

            if (!this.HasStateAuthority || _catalog == null)
                return;

            if (!_catalog.TryGetConfig(id, out PurchasableConfig config))
                return;

            if (this.Balance < config.Price)
                return;

            Vector3 spawnPosition = this.transform.position;
            if (this.Runner.TryGetPlayerObject(info.Source, out NetworkObject buyer))
            {
                // У игрока есть отдельное вложенное тело персонажа (MoveComponent живёт на нём,
                // а не на корневом PlayerNetwork) - берём позицию именно тела, а не корня.
                spawnPosition = buyer.TryGetBehaviour(out PlayerInputController inputController) && inputController.Character != null
                    ? inputController.Character.transform.position
                    : buyer.transform.position;
            }

            // Сначала спавн, потом списание - чтобы деньги не ушли, если спавн не удался.
            this.Runner.Spawn(config.Prefab, spawnPosition, Quaternion.identity);
            this.Balance -= config.Price;
        }

        private void InvokeBalanceChanged(NetworkBehaviourBuffer previousSnapshot)
        {
            this.OnBalanceChanged?.Invoke(this.Balance);
        }
    }
}
