using UnityEngine;

namespace Game
{
    public sealed class PortalPointService : MonoBehaviour
    {
        [SerializeField]
        private Transform _portal;

        public Transform Portal => _portal;
    }
}
