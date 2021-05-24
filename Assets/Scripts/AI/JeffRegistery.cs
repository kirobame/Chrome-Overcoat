using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class JeffRegistery : MonoBehaviour, ILifebound
    {
        public bool IsActive => isRegistered;
        private bool isRegistered;
        bool IActive<ILifebound>.IsActive => true;

        public event Action<ILifebound> onDestruction;

        public void Bootup() => Registery();
        public void Shutdown() => Unregistery();

        public void Registery()
        {
            //Debug.Log("Registery");
            if (Blackboard.Global.TryGet<Dictionary<Transform, bool>>("AI.Jeffs", out var jeffs))
            {
                jeffs.Add(this.transform, false);
                Blackboard.Global.Set<Dictionary<Transform, bool>>("AI.Jeffs", jeffs);
            }
            else
            {
                var _jeffs = new Dictionary<Transform, bool>();
                _jeffs.Add(this.transform, false);
                Blackboard.Global.Set<Dictionary<Transform, bool>>("AI.Jeffs", _jeffs);
            }
            isRegistered = true;
        }

        public void Unregistery()
        {
            if (Blackboard.Global.TryGet<Dictionary<Transform, bool>>("AI.Jeffs", out var jeffs))
            {
                jeffs.Remove(this.transform);
                Blackboard.Global.Set<Dictionary<Transform, bool>>("AI.Jeffs", jeffs);
            }
            isRegistered = false;
        }
    }
}
