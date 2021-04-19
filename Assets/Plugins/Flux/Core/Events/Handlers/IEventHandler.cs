using System;
using System.Collections;

namespace Flux.Event
{
    public interface IEventHandler
    {
        void AddDependency(EventToken token);
        void RemoveDependency(Enum address, object method);
    }
}