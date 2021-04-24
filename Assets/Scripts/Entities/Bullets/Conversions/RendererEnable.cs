using UnityEngine;

namespace Chrome
{
    public class RendererEnable : IEnable
    {
        public RendererEnable(Renderer renderer) => this.renderer = renderer;
        
        private Renderer renderer;

        public void Enable(bool state) => renderer.enabled = state;
    }
}