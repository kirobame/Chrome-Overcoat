namespace Chrome
{
    public static class InputRefs
    {
        public const string BOARD = "inputs";

        public const string GAMEPLAY_MAP = "gameplay";

        public static string VIEW => $"{GAMEPLAY_MAP}.view";
        public static string JUMP => $"{GAMEPLAY_MAP}.jump";
        public static string MOVE => $"{GAMEPLAY_MAP}.move";
        public static string SPRINT => $"{GAMEPLAY_MAP}.sprint";
        public static string SHOOT => $"{GAMEPLAY_MAP}.shoot";
        public static string CAST => $"{GAMEPLAY_MAP}.cast";
        public static string PICK_WP_01 => $"{GAMEPLAY_MAP}.pickWeapon01";
        public static string PICK_WP_02 => $"{GAMEPLAY_MAP}.pickWeapon02";
    }
}