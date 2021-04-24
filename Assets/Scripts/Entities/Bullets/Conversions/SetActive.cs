﻿using UnityEngine;

namespace Chrome
{
    public class SetActive : ProxyNode
    {
        public SetActive(bool enabled, IValue<GameObject> target)
        {
            this.enabled = enabled;
            this.target = target;
        }
        
        private IValue<GameObject> target;
        private bool enabled;
        
        protected override void OnUpdate(Packet packet)
        {
            if (target.IsValid(packet)) target.Value.SetActive(enabled);
            isDone = true;
        }
    }
}