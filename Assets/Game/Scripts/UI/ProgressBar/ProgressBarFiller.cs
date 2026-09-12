using UnityEngine;

namespace Game
{
    public abstract class ProgressBarFiller : MonoBehaviour
    {
        public abstract float FillAmount { get; set; }
    }
}