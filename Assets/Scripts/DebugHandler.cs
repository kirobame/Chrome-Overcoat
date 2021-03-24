using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class DebugHandler : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

        [BoxGroup("Values"), SerializeField] private float displayTime;
        [BoxGroup("Values"), SerializeField] private Color groundColor;
        [BoxGroup("Values"), SerializeField] private Color airColor;

        private Vector3 previousPosition;

        void Update()
        {
            var color = body.IsGrounded ? groundColor : airColor;
            Debug.DrawLine(previousPosition, transform.position, color, displayTime);

            previousPosition = transform.position;
        }
    }
}