namespace Chrome.Retro
{
    public class RetFreeCover : TaskedNode
    {
        public RetFreeCover(string path) => this.path = path;
        
        private string path;

        protected override void OnUse(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            var cover = board.Get<RetCover>(path);
            
            RetCoverSystem.Free(cover);
            board.Set<RetCover>(path, null);

            isDone = true;
        }
    }
}