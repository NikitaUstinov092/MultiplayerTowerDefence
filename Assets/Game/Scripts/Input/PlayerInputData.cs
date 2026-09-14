using Fusion;
using UnityEngine;

namespace SampleGame
{
    public struct PlayerInputData : INetworkInput
    {
        public Vector2 moveDirection;
        public NetworkButtons buttons; // 32
    }
}