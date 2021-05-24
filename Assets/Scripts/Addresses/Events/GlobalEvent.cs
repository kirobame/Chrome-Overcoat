using Flux;

namespace Chrome
{
    [Address]
    public enum GlobalEvent : byte
    {
        OnStart,
        OnReset
    }
}