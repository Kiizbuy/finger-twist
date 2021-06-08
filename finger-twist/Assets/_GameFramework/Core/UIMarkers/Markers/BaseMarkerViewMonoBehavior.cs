using UnityEngine;

namespace GameFramework.UI
{
    public class BaseMarkerViewMonoBehavior : MonoBehaviour, IMarkerView
    {
        [SerializeField] private int _sizeInPixels = 32;

        public void SetPosition(ICameraView cameraView, Vector3 position)
        {
            var point = cameraView.GetWorldToScreenPoint(position);

            if (OnCameraRect(point))
            {
                var screenPixels = _sizeInPixels;
                var screenSize = new Vector2(screenPixels, screenPixels);

                ((RectTransform)transform).sizeDelta = screenSize;
                gameObject.SetActive(true);
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                gameObject.SetActive(false);
            }

            transform.localPosition = new Vector3(
                (point.x - Screen.width / 2.0f) / transform.lossyScale.x,
                (point.y - Screen.height / 2.0f) / transform.lossyScale.y, 0);
        }

        public void DestroyMarker()
        {
            Destroy(gameObject);
        }

        private bool OnCameraRect(Vector3 point)
        {
            return point.x > 0 && point.x < Screen.width && point.y > 0 && point.y < Screen.height && point.z > 0;
        }
    }
}
