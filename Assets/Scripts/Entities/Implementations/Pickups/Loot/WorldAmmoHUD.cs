using Flux.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class WorldAmmoHUD : MonoBehaviour
    {
        public RectTransform RectTransform => (RectTransform)transform;
        
        [SerializeField] private Slider ammo;

        private Transform anchor;
        private BindableGauge ammoBinding;

        //--------------------------------------------------------------------------------------------------------------/

        void Update()
        {
            transform.position = anchor.position;

            var camera = Repository.Get<Camera>(Reference.Camera);
            transform.LookAt(camera.transform.position);
        }

        public void Bind(Transform anchor, BindableGauge ammoBinding)
        {
            var canvas = Repository.Get<Canvas>(Reference.WorldCanvas);
            RectTransform.SetParent(canvas.transform, false);
            
            this.anchor = anchor;
            
            this.ammoBinding = ammoBinding;
            ammoBinding.onChange += OnAmmoChange;
            
            OnAmmoChange(ammoBinding.Value);
        }
        public void Discard()
        {
            ammoBinding.onChange -= OnAmmoChange;
            gameObject.SetActive(false);
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnAmmoChange(float value)
        {
            var ratio = ammoBinding.ComputeRatio();
            ammo.value = ratio;
        }
    }
}