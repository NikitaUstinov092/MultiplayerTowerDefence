using UnityEngine;

namespace Game
{
    public sealed class PortalService : MonoBehaviour
    {
        [SerializeField]
        private Transform _portal;

        public Transform Portal => _portal;
    }
}
