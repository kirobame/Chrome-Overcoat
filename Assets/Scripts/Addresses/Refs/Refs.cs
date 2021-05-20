namespace Chrome
{
    public static class Refs
    {
        public const string LOCK = "lock";
        public const string PACKET = "packet";
        
        public const string TYPE = "type";
        public const string ROOT = "root";
        public const string PIVOT = "pivot";
        public const string COLLIDER = "collider";
        public const string SHOOT_DIRECTION = "shootDirection";
        
        public static string FIREANCHOR => $"{PIVOT}.fireAnchor";
        public static string VIEW => $"{PIVOT}.view";
        public static string RENDERER => $"{ROOT}.renderer";
    }
}