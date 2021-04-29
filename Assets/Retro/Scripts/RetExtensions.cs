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

        public static Vector3 GetMoveInput()
        {
            var output = new Vector3();
            
            if (Input.GetKey(KeyCode.RightArrow)) output.x += 1;
            if (Input.GetKey(KeyCode.LeftArrow)) output.x -= 1;
            
            if (Input.GetKey(KeyCode.UpArrow)) output.z += 1;
            if (Input.GetKey(KeyCode.DownArrow)) output.z -= 1;

            return output;
        }
    }
}