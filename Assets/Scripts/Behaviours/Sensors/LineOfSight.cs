using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class LineOfSight : MonoBehaviour
    {
        [SerializeField] private LayerMask blockingMask;
        
        public bool CanSee(Vector3 point)
        {
            var direction = point - transform.position;
            var ray = new Ray(transform.position, direction);

            return !Physics.Raycast(ray, direction.magnitude, blockingMask);
        }
        public bool CanSee(Collider collider)
        {
            var corners = collider.bounds.GetCorners();
            return corners.Any(CanSee);
        }
    }
}