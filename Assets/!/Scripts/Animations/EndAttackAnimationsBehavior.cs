using UnityEngine;

public class EndAttackAnimationsBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.SendMessage("OnEndAttackAnimationsEnter");
    }
}
