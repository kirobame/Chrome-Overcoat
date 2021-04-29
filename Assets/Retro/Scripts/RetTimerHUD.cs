using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetTimerHUD : MonoBehaviour, ILifebound
    {
        public float ElapsedTime => minutes * 60.0f + seconds - value;
        public float Value => value;
        
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text target;

        [FoldoutGroup("Values"), SerializeField] private int minutes;
        [FoldoutGroup("Values"), SerializeField] private int seconds;

        private float value;
        private bool isActive;

        void Start()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            life.Add(this);
            
            Bootup();
        }
        
        public void Bootup()
        {
            value = minutes * 60.0f + seconds;
            isActive = true;
        }
        public void Shutdown() => isActive = false;

        void Update()
        {
            if (!isActive) return;

            value -= Time.deltaTime;
            target.text = value.ConvertToTime();

            if (value < 0.0f)
            {
                isActive = false;
                
                var playerBoard = Blackboard.Global.Get<RetPlayerBoard>(RetPlayerBoard.REF_SELF);
                var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
                life.End();
            }
        }
    }
}