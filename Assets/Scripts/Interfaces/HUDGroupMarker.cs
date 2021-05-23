using UnityEngine;

namespace Chrome
{
    [RequireComponent(typeof(RectTransform))]
    public class HUDGroupMarker : MonoBehaviour
    {
        [SerializeField] private HUDGroup value;

        void Awake()
        {
            var frame = (RectTransform)transform;
            HUDBinder.RegisterHUDGroup(value, frame);
        }
    }
}