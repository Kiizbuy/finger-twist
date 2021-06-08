using UnityEngine;

namespace GameFramework.WeaponSystem
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorAttackHandlerInitializator : MonoBehaviour
    {
        [SerializeField] private MeleeWeapon _meleeWeapon;

        private Animator _meleeAnimatorHandler;

        public void SetMeleeWeapon(MeleeWeapon value)
        {
            _meleeWeapon = value;
            BindAttackBehaviors();
        }

        private void Awake()
        {
            _meleeAnimatorHandler = GetComponent<Animator>();

            if (_meleeWeapon == null)
                _meleeWeapon = GetComponent<MeleeWeapon>();

            if (_meleeWeapon != null)
                BindAttackBehaviors();

        }

        private void BindAttackBehaviors()
        {
            foreach (var attackHandle in _meleeAnimatorHandler.GetBehaviours<AttackHandleBehavior>())
            {
                attackHandle.SetMeleeWeapon(_meleeWeapon);
            }
        }
    }
}
