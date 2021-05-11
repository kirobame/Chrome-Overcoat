using System;

namespace Chrome
{
    [Flags]
    public enum Key : byte
    {
        None = 0,
        
        Inactive = 1,
        Down = 2,
        Active = 4,
        Up = 8,
        
        On = Down | Active | Up
    }
}