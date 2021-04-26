using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetCameraControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private new CinemachineVirtualCamera camera;

        private CinemachineFramingTransposer framingTransposer;

        private float height;
        private float distance;

        void Awake()
        {
            framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            distance = framingTransposer.m_CameraDistance;

            height = transform.root.position.y;
        }

        void LateUpdate() => framingTransposer.m_CameraDistance = distance - (transform.root.position.y - height);
    }
}