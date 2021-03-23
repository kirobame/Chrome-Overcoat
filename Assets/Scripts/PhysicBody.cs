using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chrome
{
    public class PhysicBody : MonoBehaviour
    {
        public CharacterController Controller => controller;
        
        [SerializeField] private CharacterController controller;

        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;

        private bool ignore;
        private Vector3 previousPosition;
        
        void Update()
        {
            ignore = true;
            controller.Move(intent * Time.deltaTime);
            intent = Vector3.zero;
            ignore = false;

            velocity += Physics.gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (controller.isGrounded) velocity = Vector3.zero;

            Debug.DrawLine(previousPosition, transform.position, Color.red, 5.0f);
            previousPosition = transform.position;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (ignore) return;
            
            if (velocity.y > 0)
            {
                Debug.Log("EXECUTE");
                
                var length = velocity.magnitude;
                velocity += hit.normal * length;
                
                velocity.y = 0;
            }

            /*if (!controller.isGrounded) Debug.Log("HIT");
            var length = velocity.magnitude;
            velocity += hit.normal * length;

            var factor = bounceMap.Evaluate(-Vector3.Dot(hit.moveDirection, hit.normal));
            velocity = velocity.normalized * (length * factor);*/




            /*var length = velocity.magnitude;
            var check = velocity.y > 0;
            
            if (check)
            {
                Debug.DrawRay(hit.point, hit.normal * length, Color.red, 10.0f);
                Debug.Log($"HIT : {length}");
            }

            var direction = Vector3.Normalize(hit.normal - hit.moveDirection);
            velocity += direction * length;
            if (check)
            {
                Debug.Log($"RESULTING VELOCITY : {velocity.magnitude}");
                //Debug.Break();
            }*/

            //Debug.Break();

            //controller.Move(velocity);
            /*var delta = hit.moveDirection * hit.moveLength;
            var projection = Vector3.ProjectOnPlane(delta, hit.normal);

            Debug.DrawRay(hit.point, delta, Color.green, 5.0f);
            Debug.DrawRay(hit.point, projection, Color.red, 5.0f);
            Debug.DrawLine(hit.point + delta, hit.point + projection, Color.magenta, 5.0f);
            velocity = projection * (1.0f - friction);*/
        }
    }
}