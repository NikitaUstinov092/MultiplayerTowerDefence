using Fusion.Addons.Physics;
using UnityEngine;

namespace SampleGame
{
    public sealed class RigidbodyGrenade : GrenadeBase
    {
        public override void Teleport(Vector3 position, Quaternion rotation)
        {
            this.GetBehaviour<NetworkRigidbody>().Teleport(position, rotation);
        }
    }
}