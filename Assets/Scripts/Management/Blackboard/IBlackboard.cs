namespace Chrome
{
    public interface IBlackboard
    {
        BlackboardSnapshot Save();
        void Load(BlackboardSnapshot snapshot);
        
        void Remove(string path);

        void SetRaw(string path, object value);
        void Set<T>(string path, T value);
        void SetRegistry(string path, IRegistry registry);

        bool TryGetAny<T>(out IRegistry<T> registry);
        
        T Get<T>(string path);
        bool TryGet<T>(string path, out T value);
        
        TRegistry GetRegistry<TRegistry>(string path) where TRegistry : IRegistry;
        bool TryGetRegistry<TRegistry>(string path, out TRegistry registry) where TRegistry : IRegistry;
    }
}