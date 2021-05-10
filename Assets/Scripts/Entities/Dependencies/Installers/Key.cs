namespace Chrome
{
    public struct Key
    {
        public static Key Default => new Key() { State = KeyState.Inactive, previousCallbackType = InputCallbackType.Cancelled};
        
        public KeyState State { get; private set; }
        private InputCallbackType previousCallbackType;

        public void Update(InputCallbackType callbackType)
        {
            if (previousCallbackType == InputCallbackType.Cancelled && callbackType == InputCallbackType.Started)
            {
                State = KeyState.Down;
            }
            else if (previousCallbackType == InputCallbackType.Started && callbackType == InputCallbackType.Performed)
            {
                State = KeyState.Active;
            }
            else if (previousCallbackType == InputCallbackType.Performed && callbackType == InputCallbackType.Cancelled)
            {
                State = KeyState.Up;
            }
            else if (previousCallbackType == InputCallbackType.Cancelled && callbackType == InputCallbackType.Ended)
            {
                State = KeyState.Inactive;
            }

            previousCallbackType = callbackType;
        }
    }
}