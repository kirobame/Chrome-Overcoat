using Flux;

namespace Chrome.Retro
{
    [Address]
    public enum RetReference : byte
    {
        Gauge,
        WaveSpawns,
        EnemyPool,
        Game,
        PickupPool,
        Timer,
        WorldCanvas,
        HUDPool
    }
}