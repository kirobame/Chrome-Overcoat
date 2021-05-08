using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public interface IRoot : ITransform
    {
        event Action<IRoot> onAttachment;
        event Action<IRoot> onDetachment;
        
        IRoot Parent { get; }
        
        string Tag { get; }
        Packet Packet { get; }

        IReadOnlyList<IRoot> Children { get; }

        void AttachTo(Transform parent);
        void AddChild(IRoot child);
    }
}