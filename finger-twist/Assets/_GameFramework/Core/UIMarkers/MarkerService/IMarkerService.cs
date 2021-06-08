namespace GameFramework.UI
{
    public interface IMarkerService
    {
        void AddMarkerLink(IMarkerLink link);
        void RemoveMarkerLink(IMarkerLink link);
        void UpdateMarkers();
    }
}
