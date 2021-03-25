using UnityEngine;

namespace Chrome
{
    public struct MoveControl
    {
        private Vector3 velocity;
        private Vector3 delta;

        public Vector3 Process(Vector3 input, float smoothing)
        {
            delta = Vector3.SmoothDamp(delta, input, ref velocity, smoothing);
            return delta;
        }
    }
}