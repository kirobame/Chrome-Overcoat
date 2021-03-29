using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class ChargeHUD : MonoBehaviour
    {
        [SerializeField] private Image charge;
        [SerializeField] private Color min;
        [SerializeField] private Color max;

        public void Set(float ratio)
        {
            charge.color = Color.Lerp(min, max, ratio);
            charge.fillAmount = ratio / 2.0f;
        }
    }
}