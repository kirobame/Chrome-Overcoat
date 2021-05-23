using System;
using System.Collections;
using UnityEngine;

namespace Chrome
{
    public abstract class Weapon : ScriptableObject
    {
        public IBlackboard Board { get; private set; }
    
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        public virtual void Build() => Board = new Blackboard();
        public abstract IBindable[] GetBindables();
        
        public abstract void Bootup(Packet packet);
        public abstract void Actualize(Packet packet);
        public abstract void Shutdown(Packet packet);

        public virtual void AssignVisualsTo(WeaponVisual visual)
        {
            visual.Renderer.sharedMesh = mesh;
            visual.Renderer.material = material;
        }
    }
}