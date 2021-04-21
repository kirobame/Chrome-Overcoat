namespace Chrome
{
    public struct OnRootDone : IDeferred
    {
        public bool? IsReady(Packet packet, ITaskTree source) => source.IsDone;
    }
}