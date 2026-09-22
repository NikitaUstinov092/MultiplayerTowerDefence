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

        [SerializeField]
        private KeyCode _buyMineKey = KeyCode.Q;

        [SerializeField]
        private KeyCode _buyArcherKey = KeyCode.E;

        public Vector2 GetMoveDirection() =>
            new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public bool IsSprint() =>
            Input.GetKey(_sprintKey);

        public bool IsBuyMine() =>
            Input.GetKey(_buyMineKey);

        public bool IsBuyArcher() =>
            Input.GetKey(_buyArcherKey);
    }
}