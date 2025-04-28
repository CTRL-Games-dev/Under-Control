using UnityEngine;

public class SimpleAttackAnimationBehavior : StateMachineBehaviour {
    public string StateEnterCallbackMethodName = "OnAttackAnimationStart";
    public string StateExitCallbackMethodName = "OnAttackAnimationEnd";

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SendMessage(StateEnterCallbackMethodName);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SendMessage(StateExitCallbackMethodName);
    }
}
