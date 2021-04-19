using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class PulseCommand : ICommand
    {
        public PulseCommand(Node target) => this.target = target;
        
        public Node Target => target;
        [SerializeField] private Node target;
    }
}