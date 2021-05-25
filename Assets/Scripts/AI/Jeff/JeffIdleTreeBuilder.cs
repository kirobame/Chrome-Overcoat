using System;

namespace Chrome
{
    [Serializable]
    public class JeffIdleTreeBuilder : TreeBuilder
    {
        public override ITaskTree Build()
        {
            return new RootNode().Append
            (
                new Delay(0.5f)
            );
        }
    }
}