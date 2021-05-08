using System;

namespace Chrome
{
    public class Token
    {
        public event Action<Token> onConsumption;

        public void Consume()
        {
            OnConsumption();
            onConsumption?.Invoke(this);
        }
        protected virtual void OnConsumption() { }
    }
}