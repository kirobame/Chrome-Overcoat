using Flux;

namespace Chrome
{
    [Address]
    public enum GaugeEvent : byte
    {
        OnGunFired,
        OnJetpackUsed,
        OnAirControlUsed,
        OnDamageInflicted,
        OnDamageReceived,
        OnKill
    }
}