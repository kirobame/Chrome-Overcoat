using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome.Retro
{
    public class RetGauge : MonoBehaviour, ILifebound
    {
        public float Value => value;
        public float Max => max;
        
        [FoldoutGroup("Dependencies"), SerializeField] private Slider slider;
        [FoldoutGroup("Dependencies"), SerializeField] private Image fill;

        [FoldoutGroup("Values"), SerializeField] private Gradient gradient;
        [FoldoutGroup("Values"), SerializeField] private float max;

        private float value;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            //life.Add(this);
            
            Bootup();
        }

        public void Bootup()
        {
            value = 0.25f * max;
            slider.value = value / max * slider.maxValue;
        }
        public void Shutdown() { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            var playerBoard = Blackboard.Global.Get<RetPlayerBoard>(RetPlayerBoard.REF_SELF);
            if (!playerBoard.Identity.Root.gameObject.activeInHierarchy) return;
            
            value = Mathf.Clamp(value, 0.0f, max);

            var ratio = value / max;
            slider.value = ratio * slider.maxValue;
            fill.color = gradient.Evaluate(ratio);

            if (ratio <= 0.0f || ratio >= 1.0f)
            {
                var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
                life.End();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void Modify(float amount) => value += amount;
    }
}