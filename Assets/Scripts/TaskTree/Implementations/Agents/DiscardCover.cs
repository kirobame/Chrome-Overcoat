namespace Chrome
{
    public class DiscardCover : TaskNode
    {
        protected override void OnUse(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            
            var cover = board.Get<CoverSpot>(AgentRefs.COVER);
            CoverSystem.Discard(cover);
            board.Set<CoverSpot>(AgentRefs.COVER, null);

            isDone = true;
        }
    }
}