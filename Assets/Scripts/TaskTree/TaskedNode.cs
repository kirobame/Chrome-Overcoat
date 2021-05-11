using System.Collections.Generic;

namespace Chrome
{
    public abstract class TaskedNode : Node
    {
        public override bool IsDone => isDone;
        protected bool isDone;

        public override void Prepare(Packet packet)
        {
            isDone = false;
            base.Prepare(packet);
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            OnUse(packet);
            if (!IsDone) return null;

            var selection = new List<INode>();
            foreach (var child in Children)
            {
                if ((output | child.Input) != output) continue;
                
                child.Prepare(packet);
                selection.Add(child);
            }

            return selection;
        }
        protected virtual void OnUse(Packet packet) { }
    }
}