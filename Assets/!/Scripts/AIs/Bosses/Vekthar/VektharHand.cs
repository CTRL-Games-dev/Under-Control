using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class VektharHand : MonoBehaviour
{
    public enum HandState {
        Slam,
        Fist,
        Sandwitch,
        Idle,
    }
    public bool IsLeft;
    [SerializeField] private GameObject _hand;
    [SerializeField] private VektharHandCollider _slamCollider;
    [SerializeField] private VektharHandCollider _fistCollider;
    [SerializeField] private VektharHandCollider _sandwitchCollider;
    [SerializeField] private Transform _handTarget;
    private Animator _animator;
    public HandState State = HandState.Idle;
    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;
    public float Damage = 50;
    void Awake() {
        _animator = _hand.GetComponent<Animator>();
    }

    void Start() {
        disableAllColliders();
        _fistCollider.TargetHit.AddListener(onTargetHit);
        _slamCollider.TargetHit.AddListener(onTargetHit);
        _sandwitchCollider.TargetHit.AddListener(onTargetHit);
    }
    void Update() {
        if(State == HandState.Idle) {
            followTarget();
        }
    }

    private void disableAllColliders() {
        _fistCollider.Collider.enabled = false;
        _slamCollider.Collider.enabled = false;
        _sandwitchCollider.Collider.enabled = false;
    }

    private void onTargetHit(LivingEntity victim) {
        Debug.Log("Vekthar hit player!");

        victim.TakeDamage(new Damage {
            Type = DamageType.PHYSICAL,
            Value = Damage
        });
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

    private void onIdle() {
        Debug.Log("Hand is now idle");
        disableAllColliders();
    }

    private void onSlam() {
        _slamCollider.Collider.enabled = true;
    }

    private void onFist() {
        _fistCollider.Collider.enabled = true;
    }

    private void onSandwitch() {
        _sandwitchCollider.Collider.enabled = true;
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
        transform.DOMoveY(1.8f, 0.5f);
        transform.DOMoveZ(target.z-1f, 0.5f);
    }
    private IEnumerator moveSandwitch() {
        var target = Player.Instance.transform.position;
        if(IsLeft) {
            yield return transform.DOMove(new(target.x-6f, transform.position.y, target.z+2f), 1.0f).WaitForCompletion();
        } else {
            yield return transform.DOMove(new(target.x+2f, transform.position.y, target.z-6f), 1.0f).WaitForCompletion();
        }
        Tween tween = transform.DOMove(new(target.x-1f, 1.8f, target.z-1f), 0.5f);
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
}