using UnityEngine;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "PlayerInputMap",
        menuName = "SampleGame/System/New PlayerInputMap"
    )]
    public sealed class PlayerInputMap : ScriptableObject
    {
        [SerializeField]
        private KeyCode _sprintKey = KeyCode.LeftShift;

        public Vector2 GetMoveDirection() =>
            new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public bool IsSprint() =>
            Input.GetKey(_sprintKey);
    }
}