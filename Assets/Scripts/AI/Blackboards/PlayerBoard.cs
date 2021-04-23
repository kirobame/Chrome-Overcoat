using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PlayerBoard : ConcreteBoard
    {
        [FoldoutGroup("Values"), SerializeField] private Collider self;
        [FoldoutGroup("Values"), SerializeField] private Transform raypoint;
        [FoldoutGroup("Values"), SerializeField] private Transform firepoint;

        protected override void BuildBoard()
        {
            blackboard.Set("type", (byte)10);
            
            blackboard.Set("canSprint", new BusyBool());

            blackboard.Set("view", raypoint);
            blackboard.Set("view.fireAnchor", firepoint);
            
            blackboard.Set("self", self.transform);
            blackboard.Set("self.collider", self);
        }
    }
}