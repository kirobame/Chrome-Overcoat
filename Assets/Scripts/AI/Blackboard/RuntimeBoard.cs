using UnityEngine;

namespace Chrome
{
    public class RuntimeBoard : MonoBehaviour, IBlackboard, IInstaller
    {
        protected Blackboard blackboard;

        protected virtual void Awake() => blackboard = new Blackboard();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public void Remove(string path) => blackboard.Remove(path);

        public void Set<T>(string path, T value) => blackboard.Set(path, value);
        public void SetRegistry(string path, IRegistry registry) => blackboard.SetRegistry(path, registry);

        public bool TryGetAny<T>(out T value) => blackboard.TryGetAny<T>(out value);
        
        public T Get<T>(string path) => blackboard.Get<T>(path);
        public bool TryGet<T>(string path, out T value) => blackboard.TryGet<T>(path, out value);

        public TRegistry GetRegistry<TRegistry>(string path) where TRegistry : IRegistry => blackboard.Get<TRegistry>(path);
        public bool TryGetRegistry<TRegistry>(string path, out TRegistry registry) where TRegistry : IRegistry => blackboard.TryGetRegistry<TRegistry>(path, out registry);
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 0;
        
        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<IBlackboard>(this);
    }
}