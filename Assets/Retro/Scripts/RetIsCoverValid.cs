using UnityEngine;

namespace Chrome.Retro
{
    public class RetIsCoverValid : Condition
    {
        public RetIsCoverValid(IValue<RetCover> cover) => this.cover = cover;
        
        private IValue<RetCover> cover;
        
        public override bool Check(Packet packet)
        {
            if (cover.IsValid(packet))
            {
                var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
                var identity = playerBoard.Get<IIdentity>(RetPlayerBoard.REF_IDENTITY);
            
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