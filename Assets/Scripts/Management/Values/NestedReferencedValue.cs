using System;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class NestedReferencedValue<T> : IValue<T>
    {
        public NestedReferencedValue(string path) => this.path = path;

        public object RawValue => value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                
                var board = packet.Get<IBlackboard>();
                var nestedBoard = board.Get<IBlackboard>(BoardName);
                nestedBoard.SetRaw(Path, value);
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

        private Packet packet;
        private T value;

        public void FillIn(Packet packet)
        {
            this.packet = packet;
            
            var board = packet.Get<IBlackboard>();
            var nestedBoard = board.Get<IBlackboard>(BoardName);
            value = nestedBoard.Get<T>(Path);
        }

        public bool IsValid(Packet packet)
        {
            this.packet = packet;
            
            if (packet.TryGet<IBlackboard>(out var board) && board.TryGet<IBlackboard>(BoardName, out var nestedBoard))
            {
                return nestedBoard.TryGet<T>(Path, out value);
            }
            else return false;
        }
    }
}