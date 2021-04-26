using Flux;

namespace Chrome.Retro
{
    [Address]
    public enum RetEvent : byte
    {
        OnTargetSpawn,
        OnTargetDeath,
    }
}