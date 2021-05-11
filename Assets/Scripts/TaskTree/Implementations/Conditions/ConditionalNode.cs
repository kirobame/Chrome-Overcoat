using System;

namespace Chrome
{
    public abstract class ConditionalNode : TaskedNode
    {
        protected override void OnUse(Packet packet)
        {
            if (Check(packet)) output = 0b_0001;
            else output = 0b_0010;

            isDone = true;
        }
        protected abstract bool Check(Packet packet);
    }
}