using UnityEngine;

public class PlayerMainAttackAnimationBehavior : StateMachineBehaviour {
    public AttackType AttackType;

    private bool _isAttacking = false;
    private int _maxLayerIndex = -1;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(_isAttacking) return;
        if(stateInfo.fullPathHash == Animator.StringToHash("attack_heavy")) return;

        animator.SendMessage("OnAttackAnimationStart", AttackType);
        _isAttacking = true;
        _maxLayerIndex = layerIndex;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!_isAttacking) return;
        if(_maxLayerIndex != layerIndex) return;

        animator.SendMessage("OnAttackAnimationEnd", AttackType);
        _isAttacking = false;
        _maxLayerIndex = -1;
    }
}
