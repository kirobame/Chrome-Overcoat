using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class GunPart
    {
        public bool IsActive { get; protected set; }
        
        public IReadOnlyList<GunPart> FollowingParts => followingParts;
        private List<GunPart> followingParts = new List<GunPart>();

        public GunControl Control => control;
        private GunControl control;

        public virtual void Bootup(GunControl control)
        {
            IsActive = true;
            this.control = control;
        }
        public void LinkTo(IEnumerable<GunPart> parts) => followingParts.AddRange(parts);
        
        public virtual void Start(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnStart(aim, args);
            foreach (var followingPart in followingParts)
            {
                if (!followingPart.IsActive) continue;
                followingPart.Start(aim, modifiedArgs);
            }
        }
        protected virtual EventArgs OnStart(Aim aim, EventArgs args) => args;

        public virtual void Update(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnUpdate(aim, args);
            foreach (var followingPart in followingParts)
            {
                if (!followingPart.IsActive) continue;
                followingPart.Update(aim, modifiedArgs);
            }
        }
        protected virtual EventArgs OnUpdate(Aim aim, EventArgs args) => args;

        public virtual void End(Aim aim, EventArgs args)
        {
            var modifiedArgs = OnEnd(aim, args);
            foreach (var followingPart in followingParts)
            {
                if (!followingPart.IsActive) continue;
                followingPart.End(aim, modifiedArgs);
            }
        }
        protected virtual EventArgs OnEnd(Aim aim, EventArgs args) => args;
    }
}