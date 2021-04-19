using System.Collections;
using Flux;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewRemoteTaskTree", menuName = "Chrome Overcoat/Task Tree/Remote")]
    public class RemoteTaskTree : ScriptableObject, IBootable
    {
        public RootNode Root => root;
        private RootNode root;

        [SerializeReference] private ITreeBuilder builder;
        
        public void Bootup() => root = builder.Build();
    }
}