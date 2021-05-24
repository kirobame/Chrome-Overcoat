using Flux;

namespace Chrome
{
    [Address]
    public enum AreaEvent : byte
    {
        OnEnemyDeath,
        OnPlayerEntry,
        OnPlayerExit
    }
}