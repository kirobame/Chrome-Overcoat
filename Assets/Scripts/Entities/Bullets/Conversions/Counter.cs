using UnityEngine;

namespace Chrome
{
    public class Counter : ConditionalNode
    {
        public Counter(int value) => this.value = value;
        
        private int value;
        private int remaining;

        protected override void OnBootup(Packet packet) => remaining = value;

        protected override bool Check(Packet packet)
        {
            remaining--;
            return remaining < 0;
        }
    }
}