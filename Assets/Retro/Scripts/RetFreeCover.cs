namespace Chrome.Retro
{
    public class RetFreeCover : ProxyNode
    {
        public RetFreeCover(string path) => this.path = path;
        
        private string path;

        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            var cover = board.Get<RetCover>(path);
            
            RetCoverSystem.Free(cover);
            board.Set<RetCover>(path, null);

            isDone = true;
        }
    }
}