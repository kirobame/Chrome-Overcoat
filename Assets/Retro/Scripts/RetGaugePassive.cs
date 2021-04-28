using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGaugePassive : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private RetGauge gauge;
        
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve map;
        [FoldoutGroup("Values"), SerializeField] private float rate;

        void Update()
        {
            gauge.Modify(map.Evaluate(gauge.Value / gauge.Max) * rate * Time.deltaTime);
        }
    }
}