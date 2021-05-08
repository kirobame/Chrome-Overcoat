namespace Chrome
{
    public static class ChromeExtensions
    {
        public static IValue<T> Cache<T>(this T value) => new CachedValue<T>(value);
        public static IValue<T> Reference<T>(this string path, ReferenceType type = ReferenceType.Local)
        {
            switch (type)
            {
                case ReferenceType.Global:
                    return new GloballyReferencedValue<T>(path);
                
                case ReferenceType.SubGlobal:
                    return new SubGloballyReferencedValue<T>(path);
                
                case ReferenceType.Local:
                    return new LocallyReferencedValue<T>(path);
            }

            return null;
        }
    }
}