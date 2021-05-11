namespace Chrome
{
    public static class InputExtensions
    {
        public static bool IsDown(this CachedValue<Key> key) => key.Value == Key.Down;
        public static bool IsActive(this CachedValue<Key> key) => key.Value == Key.Active;
        public static bool IsUp(this CachedValue<Key> key) => key.Value == Key.Up;

        public static bool IsOn(this CachedValue<Key> key) => (key.Value & Key.On) == key.Value;
    }
}