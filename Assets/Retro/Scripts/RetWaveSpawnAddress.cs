using System;

namespace Chrome.Retro
{
    [Flags]
    public enum RetWaveSpawnAddress : byte
    {
        None = 0,
        
        A = 1,
        B = 2,
        C = 4,
        D = 8,
        E = 16
    }
}