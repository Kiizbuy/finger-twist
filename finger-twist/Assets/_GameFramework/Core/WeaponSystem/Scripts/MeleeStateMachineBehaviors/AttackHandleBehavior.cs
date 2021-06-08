using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class AttackHandleBehavior : StateMachineBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float _normalizedStartDamageTime;
        [SerializeField, Range(0f, 1f)] private float _normalizedEndDamageTime;
        [SerializeField] private bool _isHeavyAttack;

        private MeleeWeapon _meleeWeapon;

        public void SetMeleeWeapon(MeleeWeapon value) => _meleeWeapon = value;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_meleeWeapon == null)
                return;

            _meleeWeapon.IsHeavyAttack = _isHeavyAttack;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_meleeWeapon == null)
                return;

            if (stateInfo.normalizedTime % 1 >= _normalizedStartDamageTime && stateInfo.normalizedTime % 1 <= _normalizedEndDamageTime)
            {
                _meleeWeapon.Attack();
            }
            else if (stateInfo.normalizedTime % 1 > _normalizedEndDamageTime)
            {
                _meleeWeapon.StopAttack();
            }

            if (stateInfo.normalizedTime % 1 > _normalizedEndDamageTime)
            {
                _meleeWeapon.StopAttack();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_meleeWeapon == null)
                return;

            _meleeWeapon.IsHeavyAttack = false;
            _meleeWeapon.StopAttack();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
