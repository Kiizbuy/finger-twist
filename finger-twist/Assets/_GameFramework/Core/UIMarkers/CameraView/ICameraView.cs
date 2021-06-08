using UnityEngine;

namespace GameFramework.UI
{
    public interface ICameraView
    {
        Vector3 GetWorldToScreenPoint(Vector3 worldPosition);
    }
}
