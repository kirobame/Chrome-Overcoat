using UnityEngine;

namespace Chrome
{
    public class RendererEnabler : IEnabler
    {
        public RendererEnabler(Renderer renderer) => this.renderer = renderer;
        
        private Renderer renderer;

        public void Enable(bool state) => renderer.enabled = state;
    }
}