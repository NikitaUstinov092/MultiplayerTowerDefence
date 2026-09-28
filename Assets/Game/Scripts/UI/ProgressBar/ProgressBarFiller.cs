using UnityEngine;

namespace Game.Scripts.UI.ProgressBar
{
    public abstract class ProgressBarFiller : MonoBehaviour
    {
        public abstract float FillAmount { get; set; }
    }
}