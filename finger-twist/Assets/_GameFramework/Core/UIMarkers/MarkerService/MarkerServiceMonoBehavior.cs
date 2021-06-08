using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.UI
{
    [RequireComponent(typeof(MarkerFactoryMonoBehavior), typeof(CameraViewMonoBehaviour))]
    public class MarkerServiceMonoBehavior : MonoBehaviour, IMarkerService
    {
        [SerializeField] private Transform _markersPanelHolder;

        private Dictionary<IMarkerLink, IMarkerView> _markers = new Dictionary<IMarkerLink, IMarkerView>();
        private IMarkerFactory<IMarkerView> _markerFactory;
        private ICameraView _cameraView;

        private void Awake()
        {
            _markerFactory = GetComponent<MarkerFactoryMonoBehavior>();
            _cameraView = GetComponent<CameraViewMonoBehaviour>();
        }

        private void LateUpdate()
        {
            UpdateMarkers();
        }

        public void AddMarkerLink(IMarkerLink link)
        {
            if (_markers.ContainsKey(link))
                return;

            var marker = _markerFactory.CreateMarker(_markersPanelHolder);
            _markers.Add(link, marker);
        }

        public void RemoveMarkerLink(IMarkerLink link)
        {
            if (!_markers.ContainsKey(link))
                return;

            _markers[link].DestroyMarker();

            _markers.Remove(link);
        }

        public void UpdateMarkers()
        {
            foreach (var marker in _markers)
            {
                marker.Value.SetPosition(_cameraView, marker.Key.Position);
            }
        }
    }
}
