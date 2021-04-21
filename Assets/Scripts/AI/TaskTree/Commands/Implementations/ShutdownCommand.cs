namespace Chrome
{
    public class ShutdownCommand : Command
    {
        public ShutdownCommand() : base(new OnRootDone()) { }

        public override void Execute(Packet packet, ITaskTree source) => source.Shutdown(packet);
    }
}