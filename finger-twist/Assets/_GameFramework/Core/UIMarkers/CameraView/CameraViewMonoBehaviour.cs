using UnityEngine;

namespace GameFramework.UI
{
    public class CameraViewMonoBehaviour : MonoBehaviour, ICameraView
    {
        [SerializeField] private Camera _viewCamera;

        private void Awake()
        {
            if (_viewCamera != null)
                return;

            _viewCamera = Camera.main;
            if (_viewCamera == null)
                Debug.LogError("view camera field is null", this);
        }

        public Vector3 GetWorldToScreenPoint(Vector3 worldPosition)
        {
            return _viewCamera.WorldToScreenPoint(worldPosition);
        }
    }
}
