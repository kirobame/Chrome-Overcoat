using System;

namespace Chrome
{
    [Flags]
    public enum AgentDefinition : short
    {
        None = 0,
        
        Peon = 1,
        Guard = 2
    }
}