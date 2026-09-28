using UnityEngine;

namespace Game.Scripts.Input
{
    [CreateAssetMenu(
        fileName = "PlayerInputMap",
        menuName = "SampleGame/System/New PlayerInputMap"
    )]
    public sealed class PlayerInputMap : ScriptableObject
    {
        [SerializeField]
        private KeyCode _buyMineKey = KeyCode.Q;

        [SerializeField]
        private KeyCode _buyArcherKey = KeyCode.E;

        public Vector2 GetMoveDirection() =>
            new(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));

        public bool IsBuyMine() =>
            UnityEngine.Input.GetKey(_buyMineKey);

        public bool IsBuyArcher() =>
            UnityEngine.Input.GetKey(_buyArcherKey);
    }
}