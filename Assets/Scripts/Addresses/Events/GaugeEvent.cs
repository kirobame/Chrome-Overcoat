using Flux;

namespace Chrome
{
    [Address]
    public enum GaugeEvent : byte
    {
        OnGunFired,
        OnJetpackUsed,
        OnThrusterUsed,
        OnDamageInflicted,
        OnDamageReceived,
        OnKill,
        OnJump,
        OnSprint,
        OnGroundMove,
        OnAirMove
    }
}