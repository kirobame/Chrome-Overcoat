using Flux.Audio;
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

        [FoldoutGroup("Feedbacks"), SerializeField] private int threshold;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage alert;

        private float value;
        private bool isActive;

        private int cachedSeconds;

        void Start()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            //life.Add(this);
            
            Bootup();
        }
        
        public void Bootup()
        {
            cachedSeconds = 0;
            value = minutes * 60.0f + seconds;
            
            isActive = true;
        }
        public void Shutdown() => isActive = false;

        void Update()
        {
            if (!isActive) return;
            
            value -= Time.deltaTime;
            
            if (value < 0.0f)
            {
                target.text = $"00:00";
                isActive = false;
                
                var playerBoard = Blackboard.Global.Get<RetPlayerBoard>(RetPlayerBoard.REF_SELF);
                var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
                life.End();
            }
            else
            {
                var currentMinutes = Mathf.FloorToInt(value / 60.0f);
                var currentSeconds = Mathf.FloorToInt(value - currentMinutes * 60.0f);

                var stringedMinutes = currentMinutes < 10 ? $"0{currentMinutes}" : currentMinutes.ToString();
                var stringedSeconds = currentSeconds < 10 ? $"0{currentSeconds}" : currentSeconds.ToString();
                target.text = $"{stringedMinutes}:{stringedSeconds}";

                if (currentSeconds != cachedSeconds)
                {
                    cachedSeconds = currentSeconds;
                    if (currentMinutes == 0 && cachedSeconds < threshold) alert.Play();
                }
            }
        }
    }
}