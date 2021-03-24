using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JumpHandler : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        [BoxGroup("Dependencies"), SerializeField] private MoveHandler move;

        [BoxGroup("Values"), SerializeField] private float margin;
        [BoxGroup("Values"), SerializeField] private float height;
        [BoxGroup("Values"), SerializeField] private float keyDownDetection;

        [BoxGroup("Launch"), SerializeField] private AnimationCurve forceMap;
        [BoxGroup("Launch"), SerializeField] private float chargeTime;
        [BoxGroup("Launch"), SerializeField] private float charge;
        [BoxGroup("Launch"), SerializeField] private AnimationCurve moveAffectMap;
        [BoxGroup("Launch"), SerializeField] private float forward;
        [BoxGroup("Launch"), SerializeField] private float limit;

        [BoxGroup("Airborne"), SerializeField, Range(0.0f, 1.0f)] private float controlTime;
        [BoxGroup("Airborne"), SerializeField] private float airTimeLimit;
        [BoxGroup("Airborne"), SerializeField] private float resistance;
        [BoxGroup("Airborne"), SerializeField] private float stomp;
        
        private bool hasBeenLaunched;
        private float launchControl;
        private float launchTime;
        private float airTime;
        
        private float pressTime;
        private float error;
        
        void Update()
        {
            if (hasBeenLaunched)
            {
                launchTime += Time.deltaTime;
                
                if (launchTime >= launchControl)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        body.velocity += Vector3.up * (resistance * Time.deltaTime);
                        
                        airTime += Time.deltaTime;
                        if (airTime >= airTimeLimit)
                        {
                            Stomp();
                            hasBeenLaunched = false;
                            
                            return;
                        }
                    }
                    else
                    {
                        hasBeenLaunched = false;
                        return;
                    }
                }

                if (body.IsGrounded || Input.GetKeyUp(KeyCode.Space))
                {
                    Stomp();
                    hasBeenLaunched = false;
                }
                
                return;
            }
            
            error += body.IsGrounded? -Time.deltaTime : Time.deltaTime;
            error = Mathf.Clamp(error, 0.0f, margin);

            if (Input.GetKey(KeyCode.Space)) pressTime += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (error >= margin)
                {
                    EndInput();
                    return;
                }
                
                var length = Mathf.Sqrt(2.0f * height * Physics.gravity.magnitude);
                var jump = Vector3.up * length;

                if (pressTime >= keyDownDetection)
                {
                    var force = forceMap.Evaluate(Mathf.Clamp01(pressTime / chargeTime)) * charge;
                    jump *= force;
                    
                    var intent = move.Intent;
                    intent.y = 0;

                    var moveAffect = moveAffectMap.Evaluate(Mathf.Clamp01(intent.magnitude / limit));
                    jump += intent * (moveAffect * forward);

                    airTime = 0.0f;
                    launchTime = 0.0f;
                    
                    hasBeenLaunched = true;
                    launchControl = force * controlTime;
                }

                body.velocity += jump;
                EndInput();
            }
        }

        private void Stomp() => body.velocity = Vector3.down * stomp;
        
        private void EndInput()
        {
            pressTime = 0.0f;
            error = margin;
        }
    }
}