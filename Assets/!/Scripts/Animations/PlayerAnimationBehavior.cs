using UnityEngine;

public class PlayerAnimationBehavior : StateMachineBehaviour
{
    [SerializeField] private Player.AnimationState animationState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Player.Instance.SetAnimationState(animationState);
    }
}