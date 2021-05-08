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
    }
}