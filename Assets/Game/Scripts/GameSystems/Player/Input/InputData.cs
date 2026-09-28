using Fusion;
using UnityEngine;

namespace Game.Scripts.GameSystems.Player.Input
{
    public struct InputData : INetworkInput
    {
        public Vector3 moveDirection;
        public NetworkButtons buttons;
    }
}