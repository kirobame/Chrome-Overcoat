using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class JetpackHUD : MonoBehaviour
    {
        [SerializeField] private Image charge;
        [SerializeField] private Image airTime;

        [Space, SerializeField] private Image icon;
        [SerializeField] private TMP_Text cooldown;
        [SerializeField] private Image cooldownFill;

        public void IndicateCharge(float ratio) => charge.fillAmount = ratio;
        public void IndicateAirTime(float ratio) => airTime.fillAmount = ratio / 2.0f;
        public void IndicateCooldown(float value, float time)
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