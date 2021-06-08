using UnityEngine;

namespace GameFramework.UI
{
    public class MarkerFactoryMonoBehavior : MonoBehaviour, IMarkerFactory<BaseMarkerViewMonoBehavior>
    {
        [SerializeField] private BaseMarkerViewMonoBehavior markerViewReference;

        public BaseMarkerViewMonoBehavior CreateMarker(Transform parent)
        {
            return Instantiate(markerViewReference, parent);
        }
    }
}
