using System;

namespace Chrome
{
    [Serializable]
    public class PackettedValue<T> : IValue<T>
    {
        public object RawValue => value;
        public T Value => value;
        
        private T value;

        public bool IsValid(Packet packet) => packet.TryGet<T>(out value);
    }
}