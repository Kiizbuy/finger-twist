using GameFramework.Utils.Gizmos;
using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.AI
{
    public class SeeSensorAngleView : MonoBehaviour, ISeeSensor
    {
        [SerializeField] private float _seeRange = 12f;
        [SerializeField, Range(2.5f, 360f)] private float _seeAngle = 2.5f;
        [SerializeField, Required] private Transform _seeSensorPoint;
        [SerializeField] private bool _canSee;

        public float SeeRange => _seeRange;
        public float SeeAngle => _seeAngle;
        public bool CanSee => _canSee;
        public Transform SeeSensorPoint => _seeSensorPoint;

        private readonly RaycastHit[] _cachedHits = new RaycastHit[36];

        public bool SeeTarget(LayerMask seeLayerMask, out Transform seenTransform)
        {
            if (!CanSee || _seeSensorPoint == null)
            {
                seenTransform = default;
                return false;
            }

            var seeRay = new Ray(_seeSensorPoint.position, _seeSensorPoint.position);
            var hitPointsCount = Physics.SphereCastNonAlloc(seeRay, _seeRange, _cachedHits, _seeRange, seeLayerMask);
            var distanceScore = float.MaxValue;

            Transform outTransform = null;

            for (var i = 0; i < hitPointsCount; i++)
            {
                if (_cachedHits[i].transform == null)
                    continue;

                if (_cachedHits[i].transform == _seeSensorPoint)
                    continue;

                var targetTransform = _cachedHits[i].transform;
                var sqrDistance = (targetTransform.position - _seeSensorPoint.position).sqrMagnitude;
                var targetAngle = Vector3.Angle(_seeSensorPoint.forward, (targetTransform.position - _seeSensorPoint.position).normalized);

                if (targetAngle > _seeAngle)
                    continue;

                if (sqrDistance > distanceScore)
                    continue;

                distanceScore = sqrDistance;
                outTransform = targetTransform;
            }


            seenTransform = outTransform;
            return true;
        }

        private void OnDrawGizmos()
        {
            if (_seeSensorPoint == null)
                return;
            Gizmos.color = Color.yellow;
            GizmosUtils.DrawWireArc(_seeSensorPoint.position, _seeSensorPoint.forward, _seeAngle, _seeRange);
        }
    }
}
