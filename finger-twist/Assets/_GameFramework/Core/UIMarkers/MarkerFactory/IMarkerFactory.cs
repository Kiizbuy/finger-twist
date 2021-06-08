using UnityEngine;

namespace GameFramework.UI
{
    public interface IMarkerFactory<out T> where T : IMarkerView
    {
        T CreateMarker(Transform parent);
    }
}
