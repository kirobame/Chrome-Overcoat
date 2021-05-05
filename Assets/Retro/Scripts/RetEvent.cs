using Flux;

namespace Chrome.Retro
{
    [Address]
    public enum RetEvent : byte
    {
        OnGameEnd,
        OnGameLost,
        OnGameWon,
        OnScreenDisplay,
        OnGunSwitch,
        OnAmmoChange,
        OnGunFound,
        OnGunLost,
        OnGameStart
    }
}