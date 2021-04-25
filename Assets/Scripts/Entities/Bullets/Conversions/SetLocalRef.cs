namespace Chrome
{
    public class SetLocalRef<T> : ProxyNode
    {
        public SetLocalRef(string path, T value)
        {
            this.path = path;
            this.value = value;
        }
        
        private string path;
        private T value;

        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set(path, value);
            
            isDone = true;
        }
    }
}