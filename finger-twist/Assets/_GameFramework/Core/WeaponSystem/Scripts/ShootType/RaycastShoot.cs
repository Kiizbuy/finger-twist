using GameFramework.Components;
using GameFramework.Utils.Gizmos;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class RaycastShoot : IShootType
    {
        [SerializeField] private Transform _raycastOrigin;
        [SerializeField] private LayerMask _shootMask = ~0;
        [SerializeField] private ProjectileViewMover _projectileViewMover;
        [SerializeField] private float _shootRange = 3f;
        [SerializeField] private float _minMaxConeProjectileRotationOffset = 5f;

        private const float MaxProjectileLiveDistance = 400f;
        private const float MaxProjectileFlySpeed = 155f;

        private float ConeRadius => _shootRange * _minMaxConeProjectileRotationOffset / 55;

        public void ShootAndTryTakeDamage(int damage, IAttackable attackable)
        {
            if (Physics.SphereCast(_raycastOrigin.position, ConeRadius, _raycastOrigin.forward, out var hit, _shootRange, _shootMask,
                QueryTriggerInteraction.Collide))
                if (hit.collider.TryGetComponent(out IHealth healthComponent))
                    healthComponent.TakeDamage(damage, attackable);

            if (_projectileViewMover == null)
                return;

            CreateProjectileView(hit);
        }

        public void StopShoot()
        {
        }

        private void CreateProjectileView(RaycastHit hit)
        {
            var rotationAdditional = Quaternion.Euler(
                Random.Range(-_minMaxConeProjectileRotationOffset, _minMaxConeProjectileRotationOffset),
                Random.Range(-_minMaxConeProjectileRotationOffset, _minMaxConeProjectileRotationOffset),
                0);

            var projectileViewInfo =
                new ProjectileViewMoverInfo((hit.collider != null ? hit.distance : MaxProjectileLiveDistance),
                    MaxProjectileFlySpeed);
            var projectileMover = Object.Instantiate(_projectileViewMover, _raycastOrigin.position,
                _raycastOrigin.rotation * rotationAdditional);

            projectileMover.PushProjectileView(projectileViewInfo);
        }

        public void DrawGizmos()
        {
            if (_raycastOrigin == null)
                return;

            var coneFov = _minMaxConeProjectileRotationOffset;
            var leftRayRotation = Quaternion.AngleAxis(-coneFov, Vector3.up);
            var rightRayRotation = Quaternion.AngleAxis(coneFov, Vector3.up);
            var upRayRotation = Quaternion.AngleAxis(-coneFov, Vector3.right);
            var downRayRotation = Quaternion.AngleAxis(coneFov, Vector3.right);
            var leftRayDirection = leftRayRotation * _raycastOrigin.forward;
            var rightRayDirection = rightRayRotation * _raycastOrigin.forward;
            var upRayDirection = upRayRotation * _raycastOrigin.forward;
            var downRayDirection = downRayRotation * _raycastOrigin.forward;

            Gizmos.color = Color.green;
            Gizmos.DrawRay(_raycastOrigin.position, leftRayDirection * _shootRange);
            Gizmos.DrawRay(_raycastOrigin.position, rightRayDirection * _shootRange);
            Gizmos.DrawRay(_raycastOrigin.position, upRayDirection * _shootRange);
            Gizmos.DrawRay(_raycastOrigin.position, downRayDirection * _shootRange);

            GizmosUtils.DrawWireDisk(_raycastOrigin.position + _raycastOrigin.forward * _shootRange,
                                     _raycastOrigin.rotation * Quaternion.Euler(90, 0, 0),
                                            ConeRadius,
                                            Color.green);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_raycastOrigin.position, _raycastOrigin.position + _raycastOrigin.forward * _shootRange);
        }
    }
}
