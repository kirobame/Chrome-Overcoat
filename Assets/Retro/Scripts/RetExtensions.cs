using Flux.Audio;
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
        
        public static string ConvertToTime(this float value)
        {
            var currentMinutes = Mathf.FloorToInt(value / 60.0f);
            var currentSeconds = Mathf.FloorToInt(value - currentMinutes * 60.0f);

            var stringedMinutes = currentMinutes < 10 ? $"0{currentMinutes}" : currentMinutes.ToString();
            var stringedSeconds = currentSeconds < 10 ? $"0{currentSeconds}" : currentSeconds.ToString();
            return $"{stringedMinutes}:{stringedSeconds}";
        }

        public static void Toggle(this CanvasGroup value, bool state)
        {
            if (state)
            {
                value.alpha = 1.0f;
                value.interactable = true;
                value.blocksRaycasts = true;
            }
            else
            {
                value.alpha = 0.0f;
                value.interactable = false;
                value.blocksRaycasts = false;
            }
        }

        public static void Play(this AudioPackage audio) => AudioHandler.Play(audio);
    }
}