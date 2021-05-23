using System;
using System.Collections;
using Flux;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class BindableCooldown : Bindable<float>
    {
        public event Action<float> onStart;
        public event Action<float> onEnd;
        
        public BindableCooldown(HUDBinding binding, float time) : base(binding) => this.time = time;
        public BindableCooldown(HUDBinding binding, float initialValue, float time) : base(binding, initialValue) => this.time = time;

        public bool IsActive => routine != null;
        public float Time => time;
        
        [SerializeField] private float time;

        private Coroutine routine;
        
        public void Start() => routine = Routines.Start(Routine());

        private IEnumerator Routine()
        {
            Value = time;
            onStart?.Invoke(value);

            while (Value > 0.0f)
            {
                Value -= UnityEngine.Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            Value = 0.0f;
            onEnd?.Invoke(value);
            
            routine = null;
        }
    }
}