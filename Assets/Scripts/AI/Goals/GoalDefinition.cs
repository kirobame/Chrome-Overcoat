using System;

namespace Chrome
{
    [Flags]
    public enum GoalDefinition : short
    {
        None = 0,
        
        Attack = 1,
        Flee = 2,
        Seek = 4,
        Idle = 8,
    }
}