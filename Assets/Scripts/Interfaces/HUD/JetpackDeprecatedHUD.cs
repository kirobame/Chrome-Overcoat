using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Chrome
{
    public class JetpackDeprecatedHUD : MonoBehaviour, IDeprecatedHUD
    {
        [SerializeField] private Image charge;
        [SerializeField] private Image airTime;

        [Space, SerializeField] private Image icon;
        [SerializeField] private TMP_Text cooldown;
        [SerializeField] private Image cooldownFill;

        public void Refresh(object value, string tag)
        {
            switch (tag)
            {
                case ("CHARGE"):
                    IndicateCharge((float)value);
                    break;
                case ("AIRTIME"):
                    IndicateAirTime((float)value);
                    break;
                case ("COOLDOWN"):
                    KeyValuePair<float, float> pair = (KeyValuePair<float, float>)value;
                    IndicateCooldown(pair.Key, pair.Value);
                    break;
                default:
                    break;
            }
        }
        
        private void IndicateCharge(float ratio) => charge.fillAmount = ratio;
        private void IndicateAirTime(float ratio) => airTime.fillAmount = ratio / 2.0f;
        private void IndicateCooldown(float value, float time)
        {
            if (value > 0.0f)
            {
                icon.enabled = false;
                cooldown.enabled = true;

                var ratio = value / time;
                cooldownFill.fillAmount = ratio;

                var remain = Mathf.CeilToInt(value);
                cooldown.text = remain < 10 ? $"0{remain}" : remain.ToString();
            }
            else
            {
                icon.enabled = true;
                
                cooldown.enabled = false;
                cooldownFill.fillAmount = 0.0f;
            }
        }
    }
}