using System;
using System.Collections;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class RespawnControl : MonoBehaviour, ILifebound
    {
        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;
        
        [SerializeField] protected string path;
        [SerializeField] protected int index;
        [SerializeField] protected float duration;

        void OnDestroy() => onDestruction?.Invoke(this);
        
        public void Bootup() { }
        public void Shutdown() => Routines.Start(Routine());

        protected virtual IEnumerator Routine()
        {
            if (index < 0) yield break;
            yield return new WaitForSeconds(duration);

            if (!TryGetRespawnAnchor(out var anchor)) yield break;

            transform.position = anchor.position;
            gameObject.SetActive(true);
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