using Fusion;
using UnityEngine;

namespace Game.Scripts.Input
{
    public struct PlayerInputData : INetworkInput
    {
        public Vector2 MoveDirection;
        public NetworkButtons Buttons; // 32
    }
}