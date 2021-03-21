using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Chrome/Settings/Player")]
    public class PlayerSettings : ScriptableObject
    {
        [FoldoutGroup("Look"), SerializeField] private float lookSmoothing;
        
        [FoldoutGroup("Look"), Space, SerializeField] private LookControl pitchControl;
        [FoldoutGroup("Look"), SerializeField] private Vector2 pitchRange;
        
        [FoldoutGroup("Look"), Space, SerializeField] private LookControl yawControl;

        [FoldoutGroup("Move"), SerializeField] private Vector2 speed;
        
        public Vector2 ComputeRotation(Vector2 mouseDelta, Look look, ref Vector2 velocity)
        {
            var rotationDelta = new Vector2(yawControl.Impact(mouseDelta.x), pitchControl.Impact(mouseDelta.y));
            var rotation = new Vector2(look.yaw, look.pitch);

            var newRotation = rotation + rotationDelta;
            newRotation.y = Mathf.Clamp(newRotation.y, pitchRange.x, pitchRange.y);

            return Vector2.SmoothDamp(rotation, newRotation, ref velocity, lookSmoothing);
        }

        public Vector3 ComputeMovement(Vector2 moveDelta)
        {
            var move = new Vector3(moveDelta.x, 0, moveDelta.y);
            move.x *= speed.x;
            move.z *= speed.y;

            return move * Time.deltaTime;
        }
    }
}