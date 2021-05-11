using UnityEngine;

namespace Chrome
{
    public abstract class MoveNode : TaskedNode
    {
        public MoveNode(IValue<Transform> self) => this.self = self;
        
        private IValue<Transform> self;
        
        protected override void OnUse(Packet packet)
        {
            if (!self.IsValid(packet))
            {
                isDone = true;
                return;
            }
            
            if (!Execute(packet, self.Value, out var hit)) return;
            
            var board = packet.Get<IBlackboard>();
            board.Set("hit", hit);

            isDone = true;
        }

        protected abstract bool Execute(Packet packet, Transform self, out CollisionHit<Transform> hit);
    }
}