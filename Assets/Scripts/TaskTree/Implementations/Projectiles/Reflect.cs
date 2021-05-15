using UnityEngine;

namespace Chrome
{
    public class Reflect : TaskNode
    {
        public Reflect(IValue<Vector3> direction) => this.direction = direction;
        
        private IValue<Vector3> direction;
        
        protected override void OnUse(Packet packet)
        {
            if (direction.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();
                var hit = board.Get<CollisionHit<Transform>>("hit");
                
                board.Set("dir", Vector3.Normalize(Vector3.Reflect(direction.Value, hit.Normal)));
            }

            isDone = true;
        }
    }
}