using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chrome
{
    public class BoardInstaller : MonoBehaviour, IInstaller
    {
        #region Nested Types

        [Serializable]
        private class Pair
        {
            public string Path => path;
            public Object Value => value;
            
            [SerializeField] private string path;
            [SerializeField] private Object value;
        }
        #endregion
        
        public int Priority => priority;

        [SerializeField] private bool global;
        [SerializeField] private int priority;
        [SerializeField] private Pair[] pairs;
        
        public void InstallDependenciesOn(Packet packet)
        {
            var board = default(IBlackboard);
            
            if (global) board = Blackboard.Global;
            else if (!packet.TryGet<IBlackboard>(out board)) return;
            
            var setMethod = typeof(IBlackboard).GetMethod("Set");
            var parameter = new object[2];
            
            foreach (var pair in pairs)
            {
                var type = pair.Value.GetType();
                var methodInstance = setMethod.MakeGenericMethod(type);

                parameter[0] = pair.Path;
                parameter[1] = pair.Value;
                methodInstance.Invoke(board, parameter);
            }
        }
    }
}