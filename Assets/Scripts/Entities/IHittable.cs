using System;
using UnityEngine;

namespace Chrome
{
    public interface IHittable
    {
        IExtendedIdentity Identity { get; }
        void Hit(IIdentity identity, HitMotive motive, EventArgs args);
    }
}