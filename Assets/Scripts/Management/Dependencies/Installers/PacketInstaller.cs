using UnityEngine;

namespace Chrome
{
    public class PacketInstaller : MonoBehaviour, IInstaller
    {
        public int Priority => priority;

        [SerializeField] private int priority;
        [SerializeField] private Object[] values;
        
        public void InstallDependenciesOn(Packet packet)
        {
            var setMethod = typeof(Packet).GetMethod("Set");
            var parameter = new object[1];
            
            foreach (var value in values)
            {
                var type = value.GetType();
                var methodInstance = setMethod.MakeGenericMethod(type);

                parameter[0] = value;
                methodInstance.Invoke(packet, parameter);
            }
        }
    }
}