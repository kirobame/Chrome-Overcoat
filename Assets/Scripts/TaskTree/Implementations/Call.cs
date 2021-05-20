using System;
using Flux.Event;

namespace Chrome
{
    public class Call : TaskNode
    {
        public Call(Enum address) => this.address = address;
        
        private Enum address;

        protected override void OnUse(Packet packet)
        {
            Events.Call(address);
            isDone = true;
        }
    }
    public class Call<T> : TaskNode
    {
        public Call(Enum address, IValue<T> argOne)
        {
            this.address = address;
            this.argOne = argOne;
        }

        private Enum address;
        private IValue<T> argOne;

        protected override void OnUse(Packet packet)
        {
            if (argOne.IsValid(packet)) Events.ZipCall(address, argOne.Value);
            isDone = true;
        }
    }
    public class Call<T1,T2> : TaskNode
    {
        public Call(Enum address, IValue<T1> argOne, IValue<T2> argTwo)
        {
            this.address = address;
            this.argOne = argOne;
            this.argTwo = argTwo;
        }

        private Enum address;
        private IValue<T1> argOne;
        private IValue<T2> argTwo;

        protected override void OnUse(Packet packet)
        {
            if (!argOne.IsValid(packet) || !argTwo.IsValid(packet)) return;
                
            Events.ZipCall(address, argOne.Value, argTwo.Value);
            isDone = true;
        }
    }
}