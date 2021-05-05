using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Chrome.Retro
{
    public class RetImpactControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private new RetCameraControl camera;
        [FoldoutGroup("Dependencies"), SerializeField] private BaseShakeControl shake;

        [FoldoutGroup("Values"), SerializeField] private Vector2 input;
        [FoldoutGroup("Values"), SerializeField] private Vector2 displacement;
        [FoldoutGroup("Values"), SerializeField] private Vector2 strength;

        private float impact;
        private bool wasGrounded;
        
        void Update()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var playerBody = playerBoard.Get<CharacterBody>(RetPlayerBoard.REF_BODY);

            if (!wasGrounded && playerBody.IsGrounded && impact > input.x)
            {
                var ratio = Mathf.InverseLerp(input.x, input.y, impact);
                
                var strength = Mathf.Lerp(this.strength.x, this.strength.y, ratio);
                shake.Add(strength, strength);
              
                camera.offset -= Mathf.Lerp(displacement.x, displacement.y, ratio);
            }

            impact = Vector3.Project(playerBody.Delta, Vector3.down).magnitude;
            wasGrounded = playerBody.IsGrounded;
        }
    }
}