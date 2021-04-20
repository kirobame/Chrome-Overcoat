namespace Chrome
{
    public abstract class Command : ICommand
    {
        public Command(IDeferred deferred) => this.deferred = deferred;
        
        public IDeferred deferred;

        public bool? IsReady(Packet packet, ITaskTree source) => deferred.IsReady(packet, source);
        public abstract void Execute(Packet packet, ITaskTree source);
    }
}