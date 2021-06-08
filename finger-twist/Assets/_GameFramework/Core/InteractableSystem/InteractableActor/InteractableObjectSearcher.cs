using GameFramework.Utils.Gizmos;
using UnityEngine;

namespace GameFramework.InteractableSystem
{
    public interface IInteractableObjectSearcher
    {
        bool SearchNearestInteractObject(out IInteractableObject interactableObject);
    }

    public class InteractableObjectSearcher : MonoBehaviour, IInteractableObjectSearcher
    {
        [SerializeField] private float _searchRadius = 5f;
        [SerializeField, Range(2f, 360f)] private float _interactAngle;
        [SerializeField] private LayerMask _interactableLayerMask = ~0;

        private readonly RaycastHit[] _raycastHitCache = new RaycastHit[32];

        public bool SearchNearestInteractObject(out IInteractableObject interactableObject)
        {
            Transform nearestObject = null;

            var maxRaycastCount = Physics.SphereCastNonAlloc(transform.position, _searchRadius, transform.forward,
                _raycastHitCache, _searchRadius, _interactableLayerMask);

            var shortDistanceToInteract = float.MaxValue;

            for (var i = 0; i < maxRaycastCount; i++)
            {
                var direction = (transform.position - _raycastHitCache[i].transform.position);
                var sqrDistance = direction.sqrMagnitude;

                if (sqrDistance < shortDistanceToInteract && Vector3.Angle(transform.forward, direction.normalized) <= _interactAngle)
                {
                    nearestObject = _raycastHitCache[i].transform;
                    shortDistanceToInteract = sqrDistance;
                }
            }

            if (nearestObject != null && nearestObject.TryGetComponent<IInteractableObject>(out interactableObject))
                return true;

            interactableObject = null;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            GizmosUtils.DrawWireArc(transform.position, transform.forward, _interactAngle, _searchRadius);
        }
    }
}
