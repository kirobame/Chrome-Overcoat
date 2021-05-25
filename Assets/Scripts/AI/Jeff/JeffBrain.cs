using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Flux.Event;

namespace Chrome
{
    [Serializable]
    public class JeffBrain : Solver
    {
        private IGoal idleGoal;
        private IGoal attackGoal;
        
        private AreaLink selfLink;

        //--------------------------------------------------------------------------------------------------------------/

        public override void Build()
        {
            idleGoal = Owner[GoalDefinition.Idle];
            attackGoal = Owner[GoalDefinition.Attack];

            var board = Owner.Identity.Packet.Get<IBlackboard>();
            selfLink = Owner.Identity.Packet.Get<AreaLink>();
        }

        public override void Bootup()
        {
            Events.Subscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Subscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);

            if (selfLink.Area.IsPlayerInAnyBounds)
            {
                attackGoal.IsActive = true;
                idleGoal.Reset();
            }
            else
            {
                idleGoal.IsActive = true;
                attackGoal.Reset();
            }
        }

        public override void Shutdown()
        {
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);

        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnPlayerEntry(Area area)
        {
            if (area != selfLink.Area) return;
            attackGoal.IsActive = true;
            idleGoal.Reset();
        }
        void OnPlayerExit(Area area)
        {
            if (area != selfLink.Area) return;
            idleGoal.IsActive = true;
            attackGoal.Reset();
        }
    }
}
