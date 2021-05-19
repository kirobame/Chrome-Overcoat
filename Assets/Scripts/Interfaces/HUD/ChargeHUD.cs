using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Chrome
{
    public class ChargeHUD : MonoBehaviour, IHUD
    {
        [SerializeField] private Image charge;
        [SerializeField] private Color min;
        [SerializeField] private Color max;

        [Space, SerializeField] private RectTransform Treshold;

        public void Refresh(object value, string tag)
        {
            switch (tag)
            {
                case ("THRESHOLD"):
                    IndicateThreshold((float)value);
                    break;
                case ("CHARGE"):
                    Set((float)value);
                    break;
                default:
                    break;
            }
        }

        private void IndicateThreshold(float ratio)
        {
            Treshold.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - ratio * 180.0f);
        }

        private void Set(float ratio)
        {
            charge.color = Color.Lerp(min, max, ratio);
            charge.fillAmount = ratio / 2.0f;
        }
    }
}
