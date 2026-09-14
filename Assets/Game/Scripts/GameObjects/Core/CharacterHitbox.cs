using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class CharacterHitbox : Hitbox
    {
        [field: SerializeField]
        public float DamageMultiplier { get; private set; } = 1;

        [field: SerializeField]
        public bool Detachable { get; private set; } = true;
    }
}