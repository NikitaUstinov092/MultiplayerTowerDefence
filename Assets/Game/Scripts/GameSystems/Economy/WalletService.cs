using SampleGame;
using UnityEngine;

namespace Game
{
    public sealed class WalletService : MonoBehaviour
    {
        [SerializeField]
        private TeamWallet _teamWallet;

        public TeamWallet TeamWallet => _teamWallet;
    }
}
