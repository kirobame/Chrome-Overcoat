using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome.Retro
{
    public class RetEnemyHealthHUD : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private Image radial;

        [FoldoutGroup("Values"), SerializeField] private Gradient gradient;

        public void Set(float ratio)
        {
            radial.color = gradient.Evaluate(1.0f - ratio);
            radial.fillAmount = ratio;
        }
    }
}