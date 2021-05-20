using UnityEngine;

namespace Chrome
{
    public static class PhysicExtensions
    {
        public static Vector3[] GetCorners(this Bounds bounds)
        {
            return new Vector3[]
            {
                bounds.min,
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), 
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), 
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), 
                
                bounds.max,
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), 
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), 
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), 
            };
        }
        
        public static bool CanSee(this Transform transform, Vector3 point, LayerMask blockingMask)
        {
            var direction = point - transform.position;
            var ray = new Ray(transform.position, direction);

            return !Physics.Raycast(ray, direction.magnitude, blockingMask);
        }
        public static bool CanSee(this Transform transform, Collider collider, LayerMask blockingMask)
        {
            var corners = collider.bounds.GetCorners();
            foreach (var corner in corners) if (CanSee(transform, corner, blockingMask)) return true;

            return false;
        }

        public static bool CastFrom(this Collider collider, Vector3 from, Vector3 displacement, LayerMask mask)
        {
            var capsuleCollider = collider as CapsuleCollider;
            var halfHeight = capsuleCollider.height * 0.5f - capsuleCollider.radius;
            
            var p1 = from + capsuleCollider.center + Vector3.up * halfHeight;
            var p2 = from + capsuleCollider.center + Vector3.down * halfHeight;
            
            return Physics.CapsuleCast(p1, p2, capsuleCollider.radius, displacement.normalized, displacement.magnitude, mask);
        }
    }
}