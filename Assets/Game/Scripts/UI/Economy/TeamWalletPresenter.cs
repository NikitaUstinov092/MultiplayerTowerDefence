using Fusion;
using SampleGame;
using TMPro;
using UnityEngine;

namespace Game
{
    // Presenter: связывает TeamWallet (логика) и TMP_Text (вьюха HUD)
    public sealed class TeamWalletPresenter : NetworkBehaviour
    {
        [SerializeField] private TeamWallet _teamWallet;
        [SerializeField] private TMP_Text _balanceText;

        public override void Spawned()
        {
            _teamWallet.OnBalanceChanged += this.OnBalanceChanged;
            this.UpdateView(_teamWallet.Balance);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _teamWallet.OnBalanceChanged -= this.OnBalanceChanged;
        }

        private void OnBalanceChanged(int balance)
        {
            this.UpdateView(balance);
        }

        private void UpdateView(int balance)
        {
            _balanceText.text = balance.ToString();
        }
    }
}
