using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Goal : IGoal
    {
        object IAssignable.Value => Owner;
        public IAgent Owner { get; private set; }
        public GoalDefinition Definition => definition;
        
        public bool IsActive { get; set; }
        public bool IsDirty { get; set; }
        public bool IsAccomplished { get; set; }

        [SerializeField] private GoalDefinition definition;

        public void AssignTo(IAgent owner) => Owner = owner;
        
        public virtual void Reset()
        {
            IsActive = false;
            IsDirty = false;
            IsAccomplished = false;
        }

        public virtual void Evaluate() { }
    }
}