using UnityEngine;

namespace Game
{
    // Presenter: связывает PurchasableConfig (данные) и PurchaseButtonView (вьюха HUD).
    // Цена статична (из конфига), поэтому это MonoBehaviour, а не NetworkBehaviour.
    public sealed class PurchaseButtonPresenter : MonoBehaviour
    {
        [SerializeField] private PurchasableConfig _config;
        [SerializeField] private PurchaseButtonView _view;

        private void Start()
        {
            _view.SetPrice(_config.Price);
        }
    }
}
