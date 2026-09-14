using System;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;

namespace SampleGame
{
    [Serializable]
    public struct HitscanEventData : INetworkStruct
    {
        public Vector3Compressed startPosition;
        public Vector3Compressed endPosition;
        public HitType hitType;

        public Vector3 Direction => ((Vector3) this.endPosition - this.startPosition).normalized;
        public Quaternion Rotation => Quaternion.LookRotation(this.Direction);
    }
}