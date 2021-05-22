using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public interface IDeprecatedHUD
    {
        void Refresh(object value, string tag);
    }
}