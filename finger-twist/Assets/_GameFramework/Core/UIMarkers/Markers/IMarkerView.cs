using UnityEngine;

namespace GameFramework.UI
{
    public interface IMarkerView
    {
        void SetPosition(ICameraView cameraView, Vector3 position);
        void DestroyMarker();
    }
}
