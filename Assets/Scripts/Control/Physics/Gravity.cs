using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Gravity : MonoBehaviour
    {
        public Vector3 Value => bypass ? overridingForce : Physics.gravity;
        public PhysicBody Body => body;
        
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

        [FoldoutGroup("Values"), SerializeField, Range(0.0f, 5.0f)] private float affect;
        [FoldoutGroup("Values"), SerializeField] private bool bypass;
        [FoldoutGroup("Values"), ShowIf("bypass"), SerializeField] private Vector3 overridingForce;

        void FixedUpdate()
        {
            var force = (bypass ? overridingForce : Physics.gravity) * (affect * body.Mass);
            body.force += force;
        }

        public void Reset() => bypass = false;
        public void Set(Vector3 force)
        {
            bypass = true;
            overridingForce = force;
        }
    }
}