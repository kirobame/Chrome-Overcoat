using System.Collections;
using UnityEngine;

namespace Chrome
{
    public abstract class Weapon : ScriptableObject
    {
        public virtual void Build() { }
        
        public abstract void Bootup(Packet packet);
        public abstract void Actualize(Packet packet);
        public abstract void Shutdown(Packet packet);
    }
}