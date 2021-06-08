using UnityEngine;

namespace GameFramework.Components
{
    public class PlaceOnTransformPosition : IObjectPlacer
    {
        [SerializeField] private Transform _transformPoint;
        [SerializeField] private bool _useTransformPointRotation;

        public void Place(GameObject placeableObject)
        {
            placeableObject.transform.position = _transformPoint.position;
            placeableObject.transform.rotation = _useTransformPointRotation ? _transformPoint.rotation : Quaternion.identity;
        }
    }
}

