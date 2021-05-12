using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class GloballyReferencedValue<T> : IValue<T>
    {
        public GloballyReferencedValue(string path) => this.path = path;
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set => Blackboard.Global.SetRaw(path, value);
        }

        [SerializeField] private string path;
            
        private T value;

        public void FillIn(Packet packet) => value = Blackboard.Global.Get<T>(path);
        public bool IsValid(Packet packet) => Blackboard.Global.TryGet<T>(path, out value);
    }
}