namespace Chrome
{
    public class Enable : TaskNode
    {
        public Enable(bool enabled, IValue<IEnabler> enabler)
        {
            this.enabled = enabled;
            this.enabler = enabler;
        }
        
        private IValue<IEnabler> enabler;
        private bool enabled;
        
        protected override void OnUse(Packet packet)
        {
            if (enabler.IsValid(packet)) enabler.Value.Enable(enabled);
            isDone = true;
        }
    }
}