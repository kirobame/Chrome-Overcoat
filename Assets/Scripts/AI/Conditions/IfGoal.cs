using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class IfGoal : Condition
    {
        [SerializeField] private GoalDefinition definition;
        [SerializeField] private bool isActive;
        
        private IGoal goal;
        
        public override void Bootup(Packet packet)
        {
            var agent = packet.Get<Agent>();
            goal = agent[definition];
        }

        public override bool Check(Packet packet) => goal.IsActive == isActive;
    }
}