namespace Chrome
{
    public class BurstCountNode : TaskNode
    {
        protected override void OnUse(Packet packet)
        {
            if (packet.TryGet<IBlackboard>(out var bb))
            {
                var amount = bb.Get<int>("weapon.burst");
                amount--;
                
                bb.Set<int>("weapon.burst", amount);
            }

            isDone = true;
        }
    }
}