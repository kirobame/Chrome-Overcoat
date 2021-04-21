using UnityEngine;

namespace Chrome
{
    public class CollisionHit
    {
        public CollisionHit(PhysicBody source, Collider collider, Vector3 point, Vector3 normal, Vector3 delta)
        {
            Source = source;
            Collider = collider;

            Point = point;
            Normal = normal;

            Delta = delta;
        }
        
        public PhysicBody Source { get; private set; }
        public Collider Collider { get; private set; }
        
        public Vector3 Point { get; private set; }
        public Vector3 Normal { get; private set; }
        
        public Vector3 Delta { get; private set; }
    }
}