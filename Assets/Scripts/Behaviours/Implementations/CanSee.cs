namespace Chrome
{
    public class CanSee : ConditionalNode
    {
        public CanSee(IValue<PhysicBody> target, IValue<LineOfSight> lineOfSight)
        {
            this.target = target;
            this.lineOfSight = lineOfSight;
        }
        
        private IValue<PhysicBody> target;
        private IValue<LineOfSight> lineOfSight;
        
        protected override bool Check(Packet packet)
        {
            if (!lineOfSight.IsValid(packet) || !target.IsValid(packet)) return false;
            return lineOfSight.Value.CanSee(target.Value.Controller);
        }
    }
}