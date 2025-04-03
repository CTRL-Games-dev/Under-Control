using UnityEngine;

public class DealDamageAnimationBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.SendMessage("OnDealDamageAnimationEnter");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.SendMessage("OnDealDamageAnimationExit");
    }

}
