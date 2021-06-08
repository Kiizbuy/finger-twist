using GameFramework.Components;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class LaserBeamShoot : IShootType
    {
        [SerializeField] private Transform _beamStartPoint;
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField, Range(0.01f, 2f)] private float _damageTimeReceiver = 1f;
        [SerializeField] private float _lazerBeamRange;
        [SerializeField] private float _minBeamWidth = 2f;
        [SerializeField] private float _maxBeamWidth = 2f;
        [SerializeField] private bool _useReflect;
        [SerializeField] private int _maxReflectionsCount = 2;

        private const float NoHitLaserRange = 200;
        private float _currentDamageTimer;


        public void ShootAndTryTakeDamage(int damage, IAttackable attackable)
        {
            _laserLine.startWidth = _minBeamWidth;
            _laserLine.endWidth = _maxBeamWidth;

            var reflections = 0;
            var damageableObjects = new List<IHealth>();
            var reflectionPoints = new List<Vector3> { _beamStartPoint.position };
            var lastPoint = _beamStartPoint.position;
            var keepReflecting = true;

            var ray = new Ray(lastPoint, _beamStartPoint.forward);

            do
            {
                var nextPoint = ray.direction * _lazerBeamRange;

                if (Physics.Raycast(ray, out var hit, _lazerBeamRange))
                {
                    nextPoint = hit.point;

                    var incomingDirection = nextPoint - lastPoint;
                    var reflectDirection = Vector3.Reflect(incomingDirection, hit.normal);

                    ray = new Ray(nextPoint, reflectDirection);

                    lastPoint = hit.point;

                    if (hit.collider.TryGetComponent<IHealth>(out var health))
                    {
                        if (!damageableObjects.Contains(health))
                            damageableObjects.Add(health);
                    }

                    reflectionPoints.Add(nextPoint);
                    reflections++;
                }
                else
                {
                    keepReflecting = false;
                }

            } while (CanReflect(keepReflecting, reflections));


            DrawLaserBeams(reflections, reflectionPoints);

            _currentDamageTimer += Time.deltaTime;

            if (!(_currentDamageTimer >= _damageTimeReceiver))
                return;

            _currentDamageTimer = 0f;
            damageableObjects.ForEach(x => x.TakeDamage(damage, attackable));
        }

        private void DrawLaserBeams(int reflections, List<Vector3> reflectionPoints)
        {
            if (reflections < 1)
            {
                _laserLine.positionCount = 2;
                _laserLine.SetPosition(0, _beamStartPoint.position);
                _laserLine.SetPosition(1,
                    _beamStartPoint.position + _beamStartPoint.forward * _lazerBeamRange * NoHitLaserRange);
            }
            else
            {
                _laserLine.positionCount = reflectionPoints.Count;
                for (var i = 0; i < reflectionPoints.Count; i++)
                    _laserLine.SetPosition(i, reflectionPoints[i]);
            }
        }

        public void StopShoot()
        {
            _currentDamageTimer = 0f;
            _laserLine.positionCount = 0;
        }

        private bool CanReflect(bool keepReflecting, int reflections)
            => _useReflect && keepReflecting && reflections < _maxReflectionsCount;

        public void DrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_beamStartPoint.position, _beamStartPoint.position + _beamStartPoint.forward * _lazerBeamRange);
        }
    }
}
