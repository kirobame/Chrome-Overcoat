namespace Chrome
{
    public class PlayerBlackboardInstaller : BlackboardInstaller
    {
        protected override void Awake()
        {
            base.Awake();
            Blackboard.Global.Set<IBlackboard>(PlayerRefs.BOARD, this);
        }
    }
}