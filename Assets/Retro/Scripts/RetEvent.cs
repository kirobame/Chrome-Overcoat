using Flux;

namespace Chrome.Retro
{
    [Address]
    public enum RetEvent : byte
    {
        OnGameEnd,
        OnGameLost,
        OnGameWon,
        OnGunSwitch,
        OnAmmoChange,
        OnGunFound,
        OnGunLost
    }
}