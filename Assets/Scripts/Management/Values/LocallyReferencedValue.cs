using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class LocallyReferencedValue<T> : IValue<T>
    {
        public LocallyReferencedValue(string path) => this.path = path;
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set => board.SetRaw(path, value);
        }

        [SerializeField] private string path;

        private IBlackboard board;
        private T value;

        public void FillIn(Packet packet)
        {
            board = packet.Get<IBlackboard>();
            value = board.Get<T>(path);
        } 
        public bool IsValid(Packet packet)
        {
            board = packet.Get<IBlackboard>();
            return board.TryGet<T>(path, out value);
        }
    }
}