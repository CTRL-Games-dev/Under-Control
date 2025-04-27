using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class VektharHand : MonoBehaviour {
    public enum HandState {
        Slam,
        Fist,
        Sandwitch,
        Idle,
    }

    [HideInInspector] public UnityEvent<LivingEntity> TargetHit;

    public VektharBoss Vekthar;

    public bool IsLeft;
    [SerializeField] private GameObject _hand;
    [SerializeField] private Transform _handTarget;
    public HandState State = HandState.Idle;
    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    private Animator _animator;

    void Awake() {
        _animator = GetComponent<Animator>();
    }

    void Update() {
        if(State == HandState.Idle) {
            followTarget();
        }
    }

    public void Attack(HandState attack) {
        string attackName;
        switch (attack)
        {
            case HandState.Slam:
                attackName = "SlamAttack";
                break;
            case HandState.Fist:
                attackName = "FistAttack";
                break;
            case HandState.Sandwitch:
                attackName = "SandwitchAttack";
                break;
            default: 
                attackName = "FistAttack";
                break;
        }
        Debug.Log($"Current hand attack: {attackName}");
        _animator.SetTrigger(attackName);
        StartCoroutine(returnToNormal(3f));
    }

    // Vekthar should not use this function, it is for the state machine connected to animator!
    public void ChangeState(HandState newState) {
        Debug.Log($"State of a hand was changed to: {newState}");
        switch (newState) {
            case HandState.Idle:
                onIdle();
                break;
            case HandState.Slam:
                onSlam();
                StartCoroutine(returnToNormal(3));
                moveToPlayer();
                break;
            case HandState.Fist:
                onFist();
                StartCoroutine(returnToNormal(3));
                moveToPlayer();
                break;
            case HandState.Sandwitch:
                onSandwitch();
                StartCoroutine(returnToNormal(3));
                StartCoroutine(moveSandwitch());
                break;
            default:
                break;
        }
        State = newState;
    }

    public void OnEnableAttack() {
        if(!Vekthar.attacking) Vekthar.attacking = true;
    }

    public void OnDisableAttack() {
        if(Vekthar.attacking) Vekthar.attacking = false;
    }

    private void onIdle() {
        Debug.Log("Hand is now idle");
    }

    private void onSlam() {
    }

    private void onFist() {
    }

    private void onSandwitch() {
    }



    private IEnumerator returnToNormal(float time) {
        yield return new WaitForSeconds(time);
        _animator.SetTrigger("ReturnToNormal");
    }

    private void followTarget() {
        var step = 20 * Time.deltaTime; // calculate distance to move
        var fastStep = 20 * Time.deltaTime * (Vector3.Distance(transform.position, _handTarget.position) / 3);
        step = Math.Max(step, fastStep);
        transform.position = Vector3.MoveTowards(transform.position, _handTarget.position, step);
    }

    private void moveToPlayer() {
        var target = Player.Instance.transform.position;
        transform.DOMoveX(target.x-1f, 0.5f);
        transform.DOMoveY(1.4f, 0.5f);
        transform.DOMoveZ(target.z-1f, 0.5f);
    }
    private IEnumerator moveSandwitch() {
        var target = Player.Instance.transform.position;
        if(IsLeft) {
            yield return transform.DOMove(new(target.x-6f, transform.position.y, target.z+2f), 1.0f).WaitForCompletion();
        } else {
            yield return transform.DOMove(new(target.x+2f, transform.position.y, target.z-6f), 1.0f).WaitForCompletion();
        }
        Tween tween = transform.DOMove(new(target.x-1f, 1.4f, target.z-1f), 0.5f);
        yield return tween.WaitForCompletion();
    }

    // Animator is cancer
    public void ChangeStateIdle() {
        ChangeState(HandState.Idle);
    }

    public void ChangeStateFist() {
        ChangeState(HandState.Fist);
    }

    public void ChangeStateSlam() {
        ChangeState(HandState.Slam);
    }

    public void ChangeStateSandwitch() {
        ChangeState(HandState.Sandwitch);
    }

    private void OnTriggerEnter(Collider other) {
        LivingEntity victim = other.GetComponentInParent<LivingEntity>(includeInactive: true);

        if(victim == null) return;

        TargetHit.Invoke(victim);
    }
}