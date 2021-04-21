namespace Chrome
{
    public struct Addition : IModification<float>
    {
        public Addition(float modification) => this.modification = modification;
        
        private float modification;
        
        public bool Update(float initial, float value, out float output)
        {
            output = value + modification;
            return true;
        }
    }
}