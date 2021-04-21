namespace Chrome
{
    public interface ICommand : IDeferred
    {
        void Execute(Packet packet, ITaskTree source);
    }
}