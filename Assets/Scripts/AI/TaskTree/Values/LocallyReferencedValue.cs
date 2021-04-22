using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class LocallyReferencedValue<T> : IValue<T>
    {
        public LocallyReferencedValue(string path) => this.path = path;
        
        public object RawValue => value;
        public T Value => value;

        [SerializeField] private string path;
            
        private T value;

        public bool IsValid(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            return board.TryGet<T>(path, out value);
        }
    }
}