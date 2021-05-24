using TMPro;
using UnityEngine;

namespace Chrome
{
    public class TimerHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Timer;

        [SerializeField] private TMP_Text timer;

        private BindableGauge timerBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            timerBinding = bindables.Get<BindableGauge>(HUDBinding.Gauge);
            timerBinding.onChange += OnTimerChange;

            OnTimerChange(timerBinding.Value);
        }
        public override void UnbindFromCurrent() => timerBinding.onChange -= OnTimerChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnTimerChange(float value)
        {
            if (value <= 0.0f)
            {
                timer.text = "00:00";
                return;
            }

            var minutes = Mathf.FloorToInt(value / 60.0f);
            var seconds = Mathf.FloorToInt(value - minutes * 60.0f);

            var leftSide = minutes >= 10 ? minutes.ToString() : $"0{minutes.ToString()}";
            var rightSids = seconds >= 10 ? seconds.ToString() : $"0{seconds.ToString()}";
            timer.text = $"{leftSide}:{rightSids}";
        }
    }
}