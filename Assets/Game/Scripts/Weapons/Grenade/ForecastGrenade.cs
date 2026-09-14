using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class ForecastGrenade : GrenadeBase
    {
        public override void Teleport(Vector3 position, Quaternion rotation)
        {
            this.GetBehaviour<NetworkTransform>().Teleport(position, rotation);
        }
    }
}