using System;

namespace Chrome
{
    [Flags]
    public enum KeyState : byte
    {
        Inactive = 0,
        
        Down = 1,
        Active = 2,
        Up = 4,
        
        On = Down | Active | Up,
    }
}