using UnityEngine;

namespace Chrome.Retro
{
    public class RetIsCoverValid : Condition
    {
        public RetIsCoverValid(IValue<Transform> self, IValue<RetCover> cover, float maxDistance)
        {
            this.self = self;
            this.cover = cover;
            
            this.maxDistance = maxDistance;
        }

        private IValue<Transform> self;
        private IValue<RetCover> cover;
        private float maxDistance;
        
        public override bool Check(Packet packet)
        {
            if (cover.IsValid(packet) && self.IsValid(packet))
            {
                var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
                var identity = playerBoard.Get<IIdentity>(RetPlayerBoard.REF_IDENTITY);

                var distance = Vector3.Distance(identity.Root.position.Flatten(), self.Value.position.Flatten());
                if (distance > maxDistance) return false;
                
                var height = cover.Value.transform.position.y;
                var direction = identity.Root.position.Flatten(height) - cover.Value.transform.position;
                var ray = new Ray(cover.Value.transform.position, direction.normalized);

                var check= Physics.Raycast(ray, direction.magnitude, LayerMask.GetMask("Environment"));
                return check;
            }

            return false;
        }
    }
}