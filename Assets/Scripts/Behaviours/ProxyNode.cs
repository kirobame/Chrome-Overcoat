using System.Collections.Generic;

namespace Chrome
{
    public abstract class ProxyNode : Node
    {
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