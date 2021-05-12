namespace Chrome
{
    public class PlayerBoard : RuntimeBoard
    {
        protected override void Awake()
        {
            base.Awake();
            Blackboard.Global.Set<IBlackboard>(PlayerRefs.BOARD, this);
        }
    }
}