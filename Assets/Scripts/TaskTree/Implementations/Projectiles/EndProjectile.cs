namespace Chrome
{
    public class EndProjectile : TaskNode
    {
        protected override void OnUse(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set("end", true);
            
            isDone = true;
        }
    }
}