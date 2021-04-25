namespace Chrome
{
    public interface IBlackboard
    {
        void Remove(string path);

        void Set<T>(string path, T value);
        void SetRegistry(string path, IRegistry registry);

        T Get<T>(string path);
        bool TryGet<T>(string path, out T value);
        
        TRegistry GetRegistry<TRegistry>(string path) where TRegistry : IRegistry;
        bool TryGetRegistry<TRegistry>(string path, out TRegistry registry) where TRegistry : IRegistry;
    }
}