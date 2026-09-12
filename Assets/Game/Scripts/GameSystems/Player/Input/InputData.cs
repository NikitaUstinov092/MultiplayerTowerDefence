using Fusion;
using UnityEngine;

namespace Game
{
    public struct InputData : INetworkInput
    {
        public Vector3 moveDirection;
        public NetworkButtons buttons;
    }
}