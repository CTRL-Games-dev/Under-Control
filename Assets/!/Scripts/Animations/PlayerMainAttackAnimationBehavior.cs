using UnityEngine;

public class PlayerMainAttackAnimationBehavior : StateMachineBehaviour {
    public AttackType AttackType;

    private bool _isAttacking = false;

    // override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
    //     // _isAttacking = true;
    //     animator.SendMessage("OnAttackAnimationStart", AttackType);
    // }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        _isAttacking = false;
        animator.SendMessage("OnAttackAnimationEnd", AttackType);
    }
 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(_isAttacking) return;

        animator.SendMessage("OnAttackAnimationStart", AttackType);
        _isAttacking = true;
    }

    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //     if(!_isAttacking) return;

    //     animator.SendMessage("OnAttackAnimationEnd", AttackType);
    //     _isAttacking = false;
    // }
}
