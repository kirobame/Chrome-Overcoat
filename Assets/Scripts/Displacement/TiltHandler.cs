using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TiltHandler : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        
        [BoxGroup("Value"), SerializeField] private SmoothedViewRotation pitch;
        [BoxGroup("Value"), SerializeField] private SmoothedViewRotation roll;
        
        void Update()
        {
            var velocity = body.transform.InverseTransformVector(body.velocity);
            Debug.Log($"X : {velocity} // Y : {velocity} // Z : {velocity}");
            
            var x = pitch.Process(velocity.y);
            var z = roll.Process(velocity.x);
            
            transform.localEulerAngles = new Vector3(x, 0.0f, z);
        }
    }
}