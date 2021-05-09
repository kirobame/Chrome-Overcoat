using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Gravity : MonoBehaviour, IInstaller, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        public Vector3 Force => bypass ? overridingForce : Physics.gravity;
        public PhysicBody Body => body.Value;

        [FoldoutGroup("Values"), SerializeField, Range(0.0f, 5.0f)] private float affect;
        [FoldoutGroup("Values"), SerializeField] private bool bypass;
        [FoldoutGroup("Values"), ShowIf("bypass"), SerializeField] private Vector3 overridingForce;

        private IValue<PhysicBody> body;

        void Awake()
        {
            body = new AnyValue<PhysicBody>();
            injections = new IValue[] { body };
        }
        
        void Update()
        {
            var force = Force * (affect * body.Value.Mass);
            body.Value.force += force;
        }

        public void Reset() => bypass = false;
        public void Set(Vector3 force)
        {
            bypass = true;
            overridingForce = force;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}