using System;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Charger : GunPart
    {
        [SerializeField] private float time;
        private float progress;

        protected override EventArgs OnStart(Aim aim, EventArgs args)
        {
            progress = 0.0f;
            return args;
        }

        protected override EventArgs OnUpdate(Aim aim, EventArgs args)
        {
            progress = aim.pressTime / time;
            progress = Mathf.Clamp01(progress);
            
            return args;
        }

        protected override EventArgs OnEnd(Aim aim, EventArgs args) => new WrapperArgs<float>(progress);
    }
}