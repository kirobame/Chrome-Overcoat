namespace Chrome
{
    public class PlayerBlackboard : BlackboardInstaller
    {
        protected override void Awake()
        {
            base.Awake();
            Blackboard.Global.Set<IBlackboard>(PlayerRefs.BOARD, this);
        }
    }
}