namespace Chrome
{
    public class Pulse : ProxyNode
    {
        public Pulse(RootNode root, Node target)
        {
            this.root = root;
            this.target = target;
        }
        
        private RootNode root;
        private Node target;

        protected override void OnUpdate(Packet packet)
        {
            root.Command(packet, new PulseCommand(target));
            isDone = true;
        }
    }
}