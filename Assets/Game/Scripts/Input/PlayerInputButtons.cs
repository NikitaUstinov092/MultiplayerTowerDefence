using System;

namespace SampleGame
{
    [Flags]
    public enum PlayerInputButtons
    {
        Sprint = 1,
        BuyMine = 2,
        BuyArcher = 4
    }
}