using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TimerControl : MonoBehaviour, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[] { lifetime };
        }

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;
        private IValue<Lifetime> lifetime;
        
        [FoldoutGroup("Values"), SerializeField] private BindableGauge time;

        //--------------------------------------------------------------------------------------------------------------/

        void Start() => HUDBinder.Declare(HUDGroup.Timer, time);

        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup() => time.Value = time.Range.y;
        public void Shutdown() { }
        
        void Update()
        {
            time.Value -= Time.deltaTime;
            if (time.IsAtMin) lifetime.Value.End();
        }
    }
}