namespace Chrome
{
    public class BlackboardSnapshot
    {
        public BlackboardSnapshot(BlackboardEntry copiedRoot) => this.copiedRoot = copiedRoot;

        public BlackboardEntry CopiedRoot => copiedRoot;
        private BlackboardEntry copiedRoot;
    }
}