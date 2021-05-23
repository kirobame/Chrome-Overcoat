using Flux;

namespace Chrome
{
    [Address]
    public enum Reference : byte
    {
        Camera,
        VirtualCamera,
        Areas,
        Pickups,
        WorldCanvas
    }
}