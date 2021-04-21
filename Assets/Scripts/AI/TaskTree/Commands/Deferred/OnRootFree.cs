namespace Chrome
{
    public struct OnRootFree : IDeferred
    {
        public bool? IsReady(Packet packet, ITaskTree source) => !source.IsLocked;
    }
}