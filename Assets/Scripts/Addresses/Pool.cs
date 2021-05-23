using Flux;

namespace Chrome
{
    [Address]
    public enum Pool : byte
    {
        Projectile,
        MuzzleFlash,
        Impact,
        Death,
        Agent,
        Modification,
        Loot,
        HUD
    }
}