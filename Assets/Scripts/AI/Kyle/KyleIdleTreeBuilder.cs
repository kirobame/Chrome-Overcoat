using System;

namespace Chrome
{
    [Serializable]
    public class KyleIdleTreeBuilder : TreeBuilder
    {
        public override ITaskTree Build()
        {
            return TT.START(GoalDefinition.Idle).Append
            (
                new Delay(0.5f)
            );
        }
    }
}