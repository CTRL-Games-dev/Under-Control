using UnityEngine;

public class PlayerAnimationBehavior : StateMachineBehaviour
{
    [SerializeField] private Player.AnimationState _animationState;
    [SerializeField] private int _slashIndex = -1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Player.Instance.SetAnimationState(_animationState, _slashIndex);
    }
}