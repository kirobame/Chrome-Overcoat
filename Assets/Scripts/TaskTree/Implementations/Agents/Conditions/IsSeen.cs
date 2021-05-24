using UnityEngine;

namespace Chrome
{
    public class IsSeen : Condition
    {
        public IsSeen(IValue<Renderer> source) => this.source = source;

        private IValue<Renderer> source;

        public override bool Check(Packet packet)
        {
            if (!source.IsValid(packet)) return false;
            return source.Value.isVisible;
        }
    }
}