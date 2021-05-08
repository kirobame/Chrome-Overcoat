using System;

namespace Chrome
{
    [Serializable]
    public class AnyValue<T> : IValue<T>
    {
        public object RawValue { get; }
        public T Value { get; }

        private T value;
        
        public void FillIn(Packet packet)
        {
            if (packet.TryGet<T>(out value)) return;
            if (packet.TryGet<IBlackboard>(out var board) && board.TryGetAny<T>(out value)) return;
        }
        public bool IsValid(Packet packet)
        {
            if (packet.TryGet<T>(out value)) return true;
            if (packet.TryGet<IBlackboard>(out var board) && board.TryGetAny<T>(out value)) return true;

            return false;
        }
    }
}