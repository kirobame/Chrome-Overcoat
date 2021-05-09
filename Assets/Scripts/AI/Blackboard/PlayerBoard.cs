namespace Chrome
{
    public class PlayerBoard : RuntimeBoard
    {
        protected override void Awake() => Blackboard.Global.Set<IBlackboard>("player", this);
    }
}