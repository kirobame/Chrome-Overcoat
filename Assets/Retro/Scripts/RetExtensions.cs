using UnityEngine;

namespace Chrome.Retro
{
    public static class RetExtensions
    {
        public static Vector3 Flatten(this Vector3 value, float height = 0.0f)
        {
            value.y = height;
            return value;
        }
    }
}