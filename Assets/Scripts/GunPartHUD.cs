using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class GunPartHUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image selection;

        public void Activate()
        {
            group.alpha = 1.0f;
            selection.enabled = true;
        }
        public void Deactivate()
        {
            group.alpha = 0.75f;
            selection.enabled = false;
        }
    }
}