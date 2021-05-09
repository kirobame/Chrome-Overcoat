using System;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class SubGloballyReferencedValue<T> : IValue<T>
    {
        public SubGloballyReferencedValue(string path) => this.path = path;
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set
            {
                var board = Blackboard.Global.Get<IBlackboard>(BoardName);
                board.SetRaw(Path, value);
            }
        }
        
        public string BoardName => path.Split('.').First();
        public string Path
        {
            get
            {
                var index = path.IndexOf('.') + 1;
                return path.Substring(index, path.Length - index);
            }
        }
        
        [SerializeField] private string path;
        
        private T value;

        public void FillIn(Packet packet)
        {
            var board = Blackboard.Global.Get<IBlackboard>(BoardName);
            value = board.Get<T>(Path);
        }
        public bool IsValid(Packet packet)
        {
            if (Blackboard.Global.TryGet<IBlackboard>(BoardName, out var board)) return board.TryGet<T>(Path, out value);
            else return false;
        }
    }
}