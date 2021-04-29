using UnityEngine;

namespace Chrome.Retro
{
    public class RetHasCover : Condition
    {
        public RetHasCover(IValue<Transform> self, string path, RetCoverProfile profile)
        {
            this.self = self;

            this.path = path;
            this.profile = profile;
        }
        
        private IValue<Transform> self;
        
        private string path;
        private RetCoverProfile profile;

        public override void Bootup(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set<RetCover>(path, null);
        }

        public override bool Check(Packet packet)
        {
            if (self.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();
                var cover = board.Get<RetCover>(path);

                if (cover != null) return true;
                else if (RetCoverSystem.Request(profile, self.Value.position, out cover))
                {
                    board.Set(path, cover);
                    return true;
                }
            }

            return false;
        }
    }
}