using Fusion;
using UnityEngine;

namespace SampleGame
{
    public struct PlayerInputData : INetworkInput
    {
        public Vector2 MoveDirection;
        public NetworkButtons Buttons; // 32
    }
}