using System;
using Flux.Data;

namespace Chrome
{
    [Serializable]
    public class AnyValue<T> : IValue<T>
    {
        #region Nested Types

        private enum Mode : byte
        {
            Packet,
            ThoroughPacket,
            LocalBoard,
            GlobalBoard,
            Repository
        }
        #endregion
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set
            {
                switch (mode)
                {
                    case Mode.Packet: ((Packet)helper).Set<T>(value);
                        break;
                    
                    case Mode.ThoroughPacket:
                        
                        var tuple = ((Packet Packet, Type type))helper;
                        tuple.Packet.Set(tuple.type, value);
                        break;
                    
                    case Mode.LocalBoard: ((IRegistry<T>)helper).Set(value);
                        break;
                    
                    case Mode.GlobalBoard: ((IRegistry<T>)helper).Set(value);
                        break;
                    
                    case Mode.Repository: Repository.Set((Enum)helper, value);
                        break;
                }
            }
        }

        private Mode mode;
        private object helper;
        
        private T value;

        public void FillIn(Packet packet) => IsValid(packet);
        public bool IsValid(Packet packet)
        {
            if (packet.TryGet<T>(out value))
            {
                mode = Mode.Packet;
                helper = packet;
                
                return true;
            }

            if (packet.TryGetThoroughly<T>(out var kvp))
            {
                mode = Mode.ThoroughPacket;
                
                value = kvp.Value;
                helper = (packet, kvp.Key);

                return true;
            }

            if (packet.TryGet<IBlackboard>(out var board) && board.TryGetAny<T>(out var registry))
            {
                mode = Mode.LocalBoard;

                value = registry.Value;
                helper = registry;
                
                return true;
            }
            
            if (Blackboard.Global.TryGetAny<T>(out registry))
            {
                mode = Mode.GlobalBoard;

                value = registry.Value;
                helper = registry;
                
                return true;
            }

            if (Repository.TryGetAny<T>(out var repoKvp))
            {
                mode = Mode.Repository;
                helper = repoKvp.Key;
                
                return true;
            }

            return false;
        }
    }
}