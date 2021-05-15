using System;

namespace Chrome
{
    [Serializable]
    public class PackettedValue<T> : IValue<T>
    {
        public object RawValue => value;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                packet.Set(value);
            }
        }

        private Packet packet;
        private T value;

        public void FillIn(Packet packet)
        {
            this.packet = packet;
            value = packet.Get<T>();
        } 

        public bool IsValid(Packet packet)
        {
            this.packet = packet;
            return packet.TryGet<T>(out value);
        }
    }
}