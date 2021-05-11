using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chrome
{
    public interface IInjectable
    {
        IReadOnlyList<IValue> Injections { get; }
    }
}