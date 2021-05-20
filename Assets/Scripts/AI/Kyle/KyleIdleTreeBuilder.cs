using System;

namespace Chrome
{
    [Serializable]
    public class KyleIdleTreeBuilder : ITreeBuilder
    {
        public ITaskTree Build()
        {
            return TT.START(GoalDefinition.Idle).Append
            (
                new Delay(0.5f)
            );
        }
    }
}