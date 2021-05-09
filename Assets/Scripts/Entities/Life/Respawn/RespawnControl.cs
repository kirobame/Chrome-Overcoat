using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class RespawnControl : MonoBehaviour, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;
        
        [SerializeField] protected string path;
        [SerializeField] protected int index;
        [SerializeField] protected float duration;

        private IValue<IRoot> root; 
        
        void Awake()
        {
            root = new AnyValue<IRoot>();
            injections = new IValue[] { root };
        }
        void OnDestroy() => onDestruction?.Invoke(this);
        
        public void Bootup() { }
        public void Shutdown() => Routines.Start(Routine());

        protected virtual IEnumerator Routine()
        {
            if (index < 0) yield break;
            yield return new WaitForSeconds(duration);

            if (!TryGetRespawnAnchor(out var anchor)) yield break;

            root.Value.Transform.position = anchor.position;
            root.Value.Transform.gameObject.SetActive(true);
        }

        protected bool TryGetRespawnAnchor(out Transform anchor)
        {
            if (!Blackboard.Global.TryGet<Transform[]>(path, out var anchors) || index >= anchors.Length)
            {
                anchor = null;
                return false;
            }
            else
            {
                anchor = anchors[index];
                return true;
            }
        }
    }
}