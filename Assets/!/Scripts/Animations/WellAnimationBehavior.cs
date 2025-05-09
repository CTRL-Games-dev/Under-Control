using UnityEngine;

public class WellAnimationBehavior : StateMachineBehaviour
{
  [SerializeField] private int _animationIndex = 0;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SendMessage("OnAnimationEnd", _animationIndex);
    }
}