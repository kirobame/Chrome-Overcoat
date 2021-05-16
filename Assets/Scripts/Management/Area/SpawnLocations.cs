using System;

namespace Chrome
{
    [Flags]
    public enum SpawnLocations 
    {
        None = 0,
        
        Alpha = 1,
        Bravo = 2,
        Charlie = 4,
        Delta = 8,
        Echo = 16,
        Foxtrot = 32,
        Golf = 64,
        Hotel = 128,
        India = 256,
        Juliet = 512,
        Kilo = 1_024,
        Lima = 2_048,
        Mike = 4_096,
        November = 8_192,
        Oscar = 16_384,
        Papa = 32_768,
        Quebec = 65_536,
        Romeo = 131_072,
        Sierra = 262_144,
        Tango = 524_288,
        Uniform = 1_048_576,
        Victor = 2_097_152,
        Whiskey = 4_194_304,
        Xray = 8_388_608,
        Yankee = 16_777_216,
        Zulu = 33_554_432
    }
}