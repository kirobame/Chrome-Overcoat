using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class ChargeHUD : MonoBehaviour
    {
        [SerializeField] private Image charge;
        [SerializeField] private Color min;
        [SerializeField] private Color max;

        [Space, SerializeField] private RectTransform Treshold;

        public void IndicateThreshold(float ratio)
        {
            Treshold.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - ratio * 180.0f);
        }
        
        public void Set(float ratio)
        {
            charge.color = Color.Lerp(min, max, ratio);
            charge.fillAmount = ratio / 2.0f;
        }
    }
}