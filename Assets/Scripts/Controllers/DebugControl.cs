using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class DebugControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;

        [FoldoutGroup("Ground"), SerializeField] private Color minGroundColor;
        [FoldoutGroup("Ground"), SerializeField] private Color maxGroundColor;
        [FoldoutGroup("Ground"), SerializeField] private Vector2 groundRange;
        
        [FoldoutGroup("Airborne"), SerializeField] private Color minAirColor;
        [FoldoutGroup("Airborne"), SerializeField] private Color maxAirColor;
        [FoldoutGroup("Airborne"), SerializeField] private Vector2 airRange;
        
        private Vector3 previousPosition;
        
        void Update()
        {
            var delta = body.Delta;
            
            if (body.IsGrounded) Display(delta, minGroundColor, maxGroundColor, groundRange);
            else Display(delta, minAirColor, maxAirColor, airRange);
        }

        private void Display(Vector3 delta, Color min, Color max, Vector2 range)
        {
            var ratio = Mathf.InverseLerp(range.x, range.y, delta.magnitude);
            var color = Color.Lerp(min, max, ratio);

            var position = transform.position;
            Debug.DrawLine(previousPosition, position, color, 5.0f);

            previousPosition = position;
        }
    }
}