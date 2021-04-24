namespace Chrome
{
    public class End : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set("end", true);
            
            isDone = true;
        }
    }
}