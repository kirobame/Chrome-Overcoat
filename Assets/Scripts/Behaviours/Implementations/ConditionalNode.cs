namespace Chrome
{
    public abstract class ConditionalNode : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            if (Check(packet)) output = 0b_0001;
            else output = 0b_0010;

            IsDone = true;
        }
        protected abstract bool Check(Packet packet);
    }
}