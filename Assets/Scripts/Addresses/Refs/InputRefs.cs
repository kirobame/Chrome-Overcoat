namespace Chrome
{
    public static class InputRefs
    {
        public const string BOARD = "inputs";

        //--------------------------------------------------------------------------------------------------------------/

        public const string GENERAL_MAP = "general";

        public static string ESCAPE = $"{GENERAL_MAP}.escape";

        //--------------------------------------------------------------------------------------------------------------/
        
        public const string GAMEPLAY_MAP = "gameplay";

        public static string VIEW => $"{GAMEPLAY_MAP}.view";
        public static string JUMP => $"{GAMEPLAY_MAP}.jump";
        public static string MOVE => $"{GAMEPLAY_MAP}.move";
        public static string SPRINT => $"{GAMEPLAY_MAP}.sprint";
        public static string SHOOT => $"{GAMEPLAY_MAP}.shoot";
        public static string PICKUP => $"{GAMEPLAY_MAP}.pickup";
        public static string STOMP => $"{GAMEPLAY_MAP}.stomp";
    }
}