namespace Chrome
{
    public class Pulse : TaskNode
    {
        public Pulse(RootNode root, Node target)
        {
            this.root = root;
            this.target = target;
        }
        
        private RootNode root;
        private Node target;

        protected override void OnUse(Packet packet)
        {
            root.Command(packet, new PulseCommand(target));
            isDone = true;
        }
    }
}