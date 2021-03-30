using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Repeater : GunPart
    {
        [SerializeField] private float time;

        private float counter;

        public override void Bootup(GunControl control)
        {
            counter = time;
            base.Bootup(control);
        }

        protected override EventArgs OnStart(Aim aim, EventArgs args)
        {
            MoveControl.canShowWalk = false;
            return args;
        }

        protected override EventArgs OnUpdate(Aim aim, EventArgs args)
        {
            counter -= Time.deltaTime;
            
            if (counter <= 0)
            {
                counter = time + counter;
                foreach (var followingPart in FollowingParts)
                {
                    if (!followingPart.IsActive) continue;
                    followingPart.End(aim, args);
                }
            }
            
            return args;
        }

        public override void End(Aim aim, EventArgs args)
        {
            OnEnd(aim, args);
            return;
        }
        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            MoveControl.canShowWalk = true;
            return args;
        }
    }
}