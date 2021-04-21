namespace Chrome
{
    public struct OnBranchDone : IDeferred
    {
        public OnBranchDone(int key) => this.key = key;
        
        private int key;

        public bool? IsReady(Packet packet, ITaskTree source)
        {
            bool? check = null;
            foreach (var branch in source.Branches)
            {
                if ((key | branch.Key) != key) continue;

                check = true;
                if (!branch.IsDone)
                {
                    check = false;
                    break;
                }
            }

            return check;
        }
    }
}