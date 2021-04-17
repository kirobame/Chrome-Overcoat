using System.Collections.Generic;

namespace Chrome
{
    public abstract class ProxyNode : Node
    {
        public override void Start(Packet packet)
        {
            IsDone = false;
            OnStart(packet);
        }
        protected virtual void OnStart(Packet packet) { }
        
        public override IEnumerable<Node> Update(Packet packet)
        {
            OnUpdate(packet);
            if (!IsDone) return null;

            var selection = new List<Node>();
            foreach (var child in Childs)
            {
                if ((output | child.Input) != output) continue;
                
                child.Start(packet);
                selection.Add(child);
            }

            return selection;
        }
        protected virtual void OnUpdate(Packet packet) { }

        protected void End(int output)
        {
            IsDone = true;
            this.output = output;
        }
    }
}