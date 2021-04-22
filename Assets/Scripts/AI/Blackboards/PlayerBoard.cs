using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class ConcreteBoard : IBlackboard
    {
        protected Blackboard blackboard;

        void Awake()
        {
            blackboard = new Blackboard();
            BuildBoard();
        }

        protected abstract void BuildBoard();

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Remove(string path) => blackboard.Remove(path);

        public void Set<T>(string path, T value) => blackboard.Set(path, value);
        public void SetRegistry(string path, IRegistry registry) => blackboard.SetRegistry(path, registry);

        public T Get<T>(string path) => blackboard.Get<T>(path);
        public bool TryGet<T>(string path, out T value) => blackboard.TryGet<T>(path, out value);

        public TRegistry GetRegistry<TRegistry>(string path) where TRegistry : IRegistry => blackboard.Get<TRegistry>(path);
        public bool TryGetRegistry<TRegistry>(string path, out TRegistry registry) where TRegistry : IRegistry => blackboard.TryGetRegistry<TRegistry>(path, out registry);
    }
    
    public class PlayerBoard : ConcreteBoard
    {
        [FoldoutGroup("Values"), SerializeField] private Collider self;
        [FoldoutGroup("Values"), SerializeField] private Transform raypoint;
        [FoldoutGroup("Values"), SerializeField] private Transform firepoint;

        protected override void BuildBoard()
        {
            blackboard.Set("type", (byte)10);
            
            blackboard.Set("canSprint", new BusyBool());

            blackboard.Set("view", raypoint);
            blackboard.Set("view.fireAnchor", firepoint);
            
            blackboard.Set("self", self.transform);
            blackboard.Set("self.collider", self);
        }
    }
}