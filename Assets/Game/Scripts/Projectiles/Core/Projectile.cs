using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Projectile : INetworkStruct
    {
        private const int SIZE_OF_DATA = 4;

        [FieldOffset(0)] // 4 byte
        public ProjectileType type;

        [FieldOffset(4)] // 4 byte
        public int startTick;

        [FieldOffset(8)]
        private Vector3Compressed _position; // 4*3=12

        [FieldOffset(20)]
        private QuaternionCompressed _rotation; // 16

        // Custom data
        [FieldOffset(36)]
        public fixed byte _customData[SIZE_OF_DATA]; // 4 bytes

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public Vector3 Direction => (Quaternion) _rotation * Vector3.forward;

        public bool IsAlive => startTick > 0;

        public readonly ref T ReinterpretData<T>() where T : unmanaged, IProjectileData
        {
            if (sizeof(T) > sizeof(byte) * SIZE_OF_DATA)
                throw new ArgumentOutOfRangeException(nameof(T),
                    $"Type of {typeof(T).Name} too large for custom data.");

            fixed (byte* ptr = _customData)
            {
                return ref *(T*) ptr;
            }
        }
    }
}