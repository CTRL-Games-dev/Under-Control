using UnityEngine;
public class VektharHandBehaviour : StateMachineBehaviour {
    [SerializeField] private VektharHand.HandState _state;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        string methodName = "ChangeStateIdle";
        switch(_state) {
            case VektharHand.HandState.Idle:
                methodName = "ChangeStateIdle";
                break;
            case VektharHand.HandState.Fist:
                methodName = "ChangeStateFist";
                break;
            case VektharHand.HandState.Slam:
                methodName = "ChangeStateSlam";
                break;
            case VektharHand.HandState.Sandwitch:
                methodName = "ChangeStateSandwitch";
                break;
        }
        animator.SendMessageUpwards(methodName);
    }
}