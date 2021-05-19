using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chrome
{
    public class PacketInstaller : MonoBehaviour, IInstaller
    {
        public int Priority => priority;

        [SerializeField] private int priority;
        [SerializeField] private Object[] values;
        
        public void InstallDependenciesOn(Packet packet)
        {
            foreach (var value in values)
            {
                var type = value.GetType();
                packet.Set(type, value);
            }
        }
    }
}