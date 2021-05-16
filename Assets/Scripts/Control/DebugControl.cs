using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class DebugControl : MonoBehaviour, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            body = new AnyValue<CharacterBody>();
            injections = new IValue[] { body };
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Ground"), SerializeField] private Color minGroundColor;
        [FoldoutGroup("Ground"), SerializeField] private Color maxGroundColor;
        [FoldoutGroup("Ground"), SerializeField] private Vector2 groundRange;
        
        [FoldoutGroup("Airborne"), SerializeField] private Color minAirColor;
        [FoldoutGroup("Airborne"), SerializeField] private Color maxAirColor;
        [FoldoutGroup("Airborne"), SerializeField] private Vector2 airRange;

        private IValue<CharacterBody> body;
        private Vector3 previousPosition;
        
        void Update()
        {
            var delta = body.Value.Delta;
            
            if (body.Value.IsGrounded) Display(delta, minGroundColor, maxGroundColor, groundRange);
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