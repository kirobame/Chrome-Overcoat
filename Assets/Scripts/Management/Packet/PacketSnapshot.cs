using System;
using System.Collections.Generic;
using System.Linq;

namespace Chrome
{
    public class PacketSnapshot
    {
        public PacketSnapshot(IEnumerable<KeyValuePair<Type, object>> values) => this.values = values.ToArray();
        
        public IReadOnlyList<KeyValuePair<Type, object>> Values => values;
        private KeyValuePair<Type, object>[] values;
    }
}