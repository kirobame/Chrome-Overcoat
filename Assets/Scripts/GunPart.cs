using System;
using System.Collections.Generic;

namespace Chrome
{
    [Serializable]
    public abstract class GunPart
    {
        public IReadOnlyList<GunPart> FollowingParts => followingParts;
        private List<GunPart> followingParts = new List<GunPart>();

        public void LinkTo(IEnumerable<GunPart> parts) => followingParts.AddRange(parts);
        
        public virtual void Start(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnStart(aim, args);
            foreach (var followingPart in followingParts) followingPart.Start(aim, modifiedArgs);
        }
        protected virtual EventArgs OnStart(Aim aim, EventArgs args) => args;

        public virtual void Update(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnUpdate(aim, args);
            foreach (var followingPart in followingParts) followingPart.Update(aim, modifiedArgs);
        }
        protected virtual EventArgs OnUpdate(Aim aim, EventArgs args) => args;

        public virtual void End(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnEnd(aim, args);
            foreach (var followingPart in followingParts) followingPart.End(aim, modifiedArgs);
        }
        protected virtual EventArgs OnEnd(Aim aim, EventArgs args) => args;
    }
}