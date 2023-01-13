using Cinemachine;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cmCam;
        [SerializeField] private float                    zoomChangeSpeed;
        [SerializeField] private float                    zoomDuration;
        [SerializeField] private AnimationCurve           cameraZoomEase;
        [SerializeField] private float                    minZoom;
        [SerializeField] private float                    maxZoom;

        private Cinemachine3rdPersonFollow _tpFollow;

        private void Awake()
        {
            _tpFollow = cmCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        public void CameraZoomIn()
        {
            if(_tpFollow.CameraDistance >= maxZoom)
            {
                return;
            }

            _tpFollow.CameraDistance += zoomChangeSpeed;
        }

        public void CameraZoomOut()
        {
            if(_tpFollow.CameraDistance <= minZoom)
            {
                return;
            }

            _tpFollow.CameraDistance -= zoomChangeSpeed;
        }

    }
}