namespace Chrome
{
    public class Enable : ProxyNode
    {
        public Enable(bool enabled, IValue<IEnable> component)
        {
            this.enabled = enabled;
            this.component = component;
        }
        
        private IValue<IEnable> component;
        private bool enabled;
        
        protected override void OnUpdate(Packet packet)
        {
            if (component.IsValid(packet)) component.Value.Enable(enabled);
            isDone = true;
        }
    }
}