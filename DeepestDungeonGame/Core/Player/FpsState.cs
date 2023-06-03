using System;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    [Flags]
    public enum FpsState
    {
        Staying = 1,
        Running = 2,
        Walking = 4,
        Crouching = 8,
        Jumping = 16
    }
}