using UnityEngine;

namespace Chrome
{
    public struct Spring : IModification<float>
    {
        public Spring(float smoothing)
        {
            this.smoothing = smoothing;
            damping = 0.0f;
        }

        private float smoothing;
        private float damping;

        public bool Update(float initial, float value, out float output)
        {
            output = Mathf.SmoothDamp(value, initial, ref damping, smoothing);
            return false;
        }
    }
}