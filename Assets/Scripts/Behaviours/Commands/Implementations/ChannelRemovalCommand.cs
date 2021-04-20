namespace Chrome
{
    public class ChannelRemovalCommand : Command
    {
        public ChannelRemovalCommand(int removal) : base(new OnBranchDone(removal)) => this.removal = removal;

        private int removal;
        
        public override void Execute(Packet packet, ITaskTree source) => source.RemoveOutputChannel(packet, removal);
    }
}