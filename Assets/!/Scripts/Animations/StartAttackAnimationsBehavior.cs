using UnityEngine;

public class StartAttackAnimationsBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.SendMessage("OnStartAttackAnimationsEnter");
    }

}
