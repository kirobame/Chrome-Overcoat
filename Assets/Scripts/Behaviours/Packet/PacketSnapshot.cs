using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flux;

namespace Chrome
{
    public class PacketSnapshot
    {
        public PacketSnapshot(IEnumerable<object> values) => this.values = values.ToArray();
        
        public IReadOnlyList<object> Values => values;
        private object[] values;
    }
}