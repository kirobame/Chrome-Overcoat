﻿using System;

namespace Chrome.Retro
{
    [Flags]
    public enum RetWaveSpawnAddress : short
    {
        None = 0,
        
        A = 1,
        B = 2,
        C = 4,
        D = 8,
        E = 16,
        F = 32,
        G = 64,
        H = 128,
        I = 256,
    }
}