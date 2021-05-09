using System;
using Flux.Data;

namespace Chrome
{
    [Serializable]
    public class StoredValue<T> : IValue<T>
    {
        public StoredValue(Enum address) => this.address = address;
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set => Repository.Set(address, value);
        }

        private Enum address;
        private T value;
        
        public void FillIn(Packet packet) => value = Repository.Get<T>(address);
        public bool IsValid(Packet packet) => Repository.TryGet<T>(address, out value);
    }
}