using System.Collections.Generic;

namespace Chrome
{
    public abstract class ProxyNode : Node
    {
        public override bool IsDone => isDone;
        protected bool isDone;

        public override void Start(Packet packet)
        {
            isDone = false;
            base.Start(packet);
        }

        public override IEnumerable<INode> Update(Packet packet)
        {
            OnUpdate(packet);
            if (!IsDone) return null;

            var selection = new List<INode>();
            foreach (var child in Children)
            {
                if ((output | child.Input) != output) continue;
                
                child.Start(packet);
                selection.Add(child);
            }

            return selection;
        }
        protected virtual void OnUpdate(Packet packet) { }
    }
}