using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chrome
{
    public class PlayerBoard : RuntimeBoard
    {
        protected override void Awake() => Blackboard.Global.Set<IBlackboard>("player", this);
    }

    public class PlayerInstaller : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private Collider self;
        [FoldoutGroup("Values"), SerializeField] private Transform raypoint;
        [FoldoutGroup("Values"), SerializeField] private Transform firepoint;
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;
        
        public void InstallDependenciesOn(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            
            board.Set("type", (byte)10);
            board.Set("canSprint", new BusyBool());
            board.Set("view", raypoint);
            board.Set("view.fireAnchor", firepoint);
            board.Set("self", self.transform);
            board.Set("self.collider", self);
        }
    }

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
        
        [SerializeField] private int priority;
        [SerializeField] private Pair[] pairs;
        
        public void InstallDependenciesOn(Packet packet)
        {
            if (!packet.TryGet<IBlackboard>(out var board)) return;
            
            var setMethod = typeof(IBlackboard).GetMethod("Set");
            var parameter = new object[2];
            
            foreach (var pair in pairs)
            {
                var type = pair.Value.GetType();
                var methodInstance = setMethod.MakeGenericMethod(type);

                parameter[0] = pair.Path;
                parameter[1] = pair.Value;
                methodInstance.Invoke(packet, parameter);
            }
        }
    }
    
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