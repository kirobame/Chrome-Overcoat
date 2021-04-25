namespace Chrome
{
    public interface IDeferred
    {
        bool? IsReady(Packet packet, ITaskTree source);
    }
}