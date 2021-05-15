namespace Chrome
{
    public class SetLocalReference<T> : TaskNode
    {
        public SetLocalReference(string path, T value)
        {
            this.path = path;
            this.value = value;
        }
        
        private string path;
        private T value;

        protected override void OnUse(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set(path, value);
            
            isDone = true;
        }
    }
}