using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Gravity : MonoBehaviour
    {
        public PhysicBody Body => body;
        
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

        [FoldoutGroup("Values"), SerializeField, Range(0.0f, 1.0f)] private float affect;
        [FoldoutGroup("Values"), SerializeField] private bool bypass;
        [FoldoutGroup("Values"), ShowIf("bypass"), SerializeField] private Vector3 overridingForce;

        void Update()
        {
            var force = (bypass ? overridingForce : Physics.gravity) * (affect * Time.deltaTime);
            body.velocity += force;
        }

        public void Reset() => bypass = false;
        public void Set(Vector3 force)
        {
            bypass = true;
            overridingForce = force;
        }
    }
}