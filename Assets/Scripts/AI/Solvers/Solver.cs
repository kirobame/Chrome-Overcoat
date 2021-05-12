using UnityEngine;

namespace Chrome
{
    public abstract class Solver : ISolver
    {
        object IAssignable.Value => Owner;
        public IAgent Owner { get; private set; }

        public void AssignTo(IAgent owner) => Owner = owner;

        public virtual void Build() { }

        public virtual void Bootup() { }
        public virtual void Evaluate() { }
        public virtual void Shutdown() { }
    }
}