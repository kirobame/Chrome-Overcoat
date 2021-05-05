using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetCameraControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private new CinemachineVirtualCamera camera;

        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        
        [HideInInspector] public float offset;
        
        private CinemachineFramingTransposer framingTransposer;
        
        private float height;
        private float distance;
        private float damping;
        
        void Awake()
        {
            framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            distance = framingTransposer.m_CameraDistance;

            height = camera.m_Follow.position.y;
        }

        void LateUpdate()
        {
            framingTransposer.m_CameraDistance = distance - (camera.m_Follow.position.y - height + offset);
            offset = Mathf.SmoothDamp(offset, 0.0f, ref damping, smoothing);
        }
    }
}