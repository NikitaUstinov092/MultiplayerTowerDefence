using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Economy
{
    public sealed class PurchaseButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _price;

        public void SetPrice(int price)
        {
            _price.text = price.ToString();
        }
    }
}
