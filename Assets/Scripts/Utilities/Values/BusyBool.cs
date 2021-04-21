namespace Chrome
{
    public struct BusyBool
    {
        public bool Value => business <= 0;

        public int business;
    }
}