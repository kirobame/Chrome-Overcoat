using Flux;

namespace Chrome
{
    [Address]
    public enum GaugeEvent : byte
    {
        OnGunFired,
        OnFrenzyAbilityUsed,
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