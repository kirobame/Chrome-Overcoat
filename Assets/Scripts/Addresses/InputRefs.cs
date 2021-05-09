namespace Chrome
{
    public static class InputRefs
    {
        public const string BOARD = "inputs";

        public const string GAMEPLAY_MAP = "gameplay";
            
        public static string JUMP => $"{GAMEPLAY_MAP}.jump";
        public static string MOVE => $"{GAMEPLAY_MAP}.move";
        public static string SHOOT => $"{GAMEPLAY_MAP}.shoot";
    }
}