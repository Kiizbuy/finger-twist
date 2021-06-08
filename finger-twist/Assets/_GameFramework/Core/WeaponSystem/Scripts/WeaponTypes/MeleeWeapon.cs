using GameFramework.Components;
using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class MeleeWeapon : MonoBehaviour, IWeapon
    {
        [System.Serializable]
        public struct AttackPoint : IAttackable
        {
            public float Radius;
            public Vector3 Offset;
            public Transform AttackRoot;

        }

        [ReorderableList]
        [SerializeField]
        private AttackPoint[] _attackPoints = new AttackPoint[0];

        [SerializeField] private WeaponData _meleeWeaponData;
        [SerializeField] private LayerMask _damageableLayerMask = ~0;
        [SerializeField] private int _heavyAttackMultiplier = 30;
        [SerializeField, ReadOnly] private bool _inAttackState;

        private Vector3[] _previousAttackPosition;
        private Vector3 _attackDirection;

        private readonly RaycastHit[] _raycastHitCache = new RaycastHit[64];
        private readonly float _attackDistanceThreshold = 0.001f;
        private readonly float _attackOffsetMultiplier = 0.0001f;

        public bool IsHeavyAttack { get; set; }
        public int Damage => _meleeWeaponData.Damage * (IsHeavyAttack ? _heavyAttackMultiplier : 1);



        private void Awake()
        {
            UpdateAttackPointsPosition();
        }

        public void Attack()
        {
            _inAttackState = true;
            UpdateAttackPointsPosition();
        }

        private void UpdateAttackPointsPosition()
        {
            _previousAttackPosition = new Vector3[_attackPoints.Length];

            for (var i = 0; i < _attackPoints.Length; ++i)
            {
                var worldPos = _attackPoints[i].AttackRoot.position +
                               _attackPoints[i].AttackRoot.TransformVector(_attackPoints[i].Offset);

                _previousAttackPosition[i] = worldPos;
            }
        }

        public void StopAttack()
        {
            _inAttackState = false;
        }

        private void FixedUpdate()
        {
            if (!CanAttack())
                return;

            HandleMeleeDamage();
        }

        private void HandleMeleeDamage()
        {
            for (var i = 0; i < _attackPoints.Length; ++i)
            {
                var currentAttackPoint = _attackPoints[i];
                var translatedWorldPosition = currentAttackPoint.AttackRoot.position +
                                              currentAttackPoint.AttackRoot.TransformVector(currentAttackPoint.Offset);
                var attackVector = translatedWorldPosition - _previousAttackPosition[i];

                if (attackVector.magnitude < _attackDistanceThreshold)
                    attackVector = Vector3.forward * _attackOffsetMultiplier;


                var attackRay = new Ray(translatedWorldPosition, attackVector.normalized);
                var contacts = Physics.SphereCastNonAlloc(attackRay,
                    currentAttackPoint.Radius,
                    _raycastHitCache,
           attackVector.magnitude,
                    _damageableLayerMask,
                    QueryTriggerInteraction.Ignore);


                for (var k = 0; k < contacts; k++)
                {
                    var col = _raycastHitCache[k].collider;

                    if (col != null)
                        CheckDamage(col, currentAttackPoint);
                }

                _previousAttackPosition[i] = translatedWorldPosition;
            }
        }

        private void CheckDamage(Component other, AttackPoint attackPoint)
        {
            var damageableHealth = other.GetComponent<IHealth>();

            //TODO add check damage on owner
            //if (Damageable is own owner)
            //    return; 

            damageableHealth?.TakeDamage(Damage, attackPoint);

        }

        public bool CanAttack()
        {
            return _inAttackState;
        }


#if UNITY_EDITOR

        private void OnDrawGizmos()
        {

            foreach (var currentAttackPoint in _attackPoints)
            {
                if (currentAttackPoint.AttackRoot != null)
                {
                    var worldPos = currentAttackPoint.AttackRoot.TransformVector(currentAttackPoint.Offset);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(currentAttackPoint.AttackRoot.position + worldPos, currentAttackPoint.Radius);
                }
            }
        }
#endif

    }
}
