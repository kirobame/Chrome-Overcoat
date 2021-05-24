using System;

namespace Chrome
{
    [Serializable]
    public class AgentBrain : Solver
    {
        private AnyValue<Health> health;

        private IGoal fleeGoal;
        private IGoal attackGoal;
        
        public override void Build()
        {
            health = new AnyValue<Health>();
            health.FillIn(Owner.Identity.Packet);
            
            fleeGoal = Owner[GoalDefinition.Flee];
            attackGoal = Owner[GoalDefinition.Attack];
        }

        public override void Evaluate()
        {
            var percentage = health.Value.Amount / health.Value.Max;
            if (percentage > 0.25f)
            {
                if (attackGoal.IsActive) return;
                
                attackGoal.IsActive = true;
                fleeGoal.Reset();
            }
            else if (!fleeGoal.IsActive)
            {
                fleeGoal.IsActive = true;
                attackGoal.Reset();
            }
        }
    }
}