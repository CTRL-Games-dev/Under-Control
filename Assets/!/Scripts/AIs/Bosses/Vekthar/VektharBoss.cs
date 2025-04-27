using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VektharBoss : LivingEntity
{
    enum VektharState {
        WakeUp,
        Follow,
        StartAttack,
        DuringAttack,
        EndAttack,
    }
    private VektharState _state = VektharState.WakeUp;
    private Animator _eyeAnimator;
    [SerializeField] private GameObject _eyes;
    [SerializeField] private GameObject _body;
    [SerializeField] private VektharHand _handLeft;
    [SerializeField] private VektharHand _handRight;

    void Awake() {
        _eyeAnimator = GetComponent<Animator>();   
    }

    void Start() {
        _handLeft.TargetHit.AddListener(hitPlayer);
        _handRight.TargetHit.AddListener(hitPlayer);
    }

    private void hitPlayer(LivingEntity entity) {
        Attack(new Damage {
            Type = DamageType.PHYSICAL,
            Value = 50
        }, entity);
        
        Debug.Log("Player was hit!");
        _handLeft.EnableColliders(false);
        _handRight.EnableColliders(false);
    }

    void Update() {
        switch (_state)
        {
            case VektharState.WakeUp: {
                Debug.Log("Vek'thar is now following player");
                _state = VektharState.Follow;

                StartCoroutine(scheduleAttack(5));
            } break;
            case VektharState.Follow:
                follow();
                break;
            case VektharState.StartAttack:
                startAttack();
                break;
            case VektharState.DuringAttack: {
                if(_handLeft.State == VektharHand.HandState.Idle && _handRight.State == VektharHand.HandState.Idle) {
                    _state = VektharState.EndAttack;
                }
            } break;
            case VektharState.EndAttack: {
                StartCoroutine(scheduleAttack(3));
                _state = VektharState.Follow;
            } break;
        }
    }

    private void follow() {

        Vector3 target = Player.Instance.transform.position;
        target.x += 5;
        target.z += 5;
        target.y = 3;

        var step = 10 * Time.deltaTime; // calculate distance to move
        var fastStep = 10 * Time.deltaTime * (Vector3.Distance(_body.transform.position, target) / 4);
        step = Math.Max(step, fastStep);      

        _body.transform.position = Vector3.MoveTowards(_body.transform.position, target, step);
    }
    private void startAttack() {
        Debug.Log("Starting an attack.");

        int attackNumber = UnityEngine.Random.Range(0, 3);
        switch(attackNumber) {
            case 0: {
                _handRight.Attack(VektharHand.HandState.Fist);
                AudioClip fistAttack = Resources.Load("SFX/vekthar/fist") as AudioClip;
                SoundFXManager.Instance.PlaySoundFXClip(fistAttack, transform,1f);
            } break;
            case 1: {
                _handLeft.Attack(VektharHand.HandState.Sandwitch);
                _handRight.Attack(VektharHand.HandState.Sandwitch);
                AudioClip sandwitchAttack = Resources.Load("SFX/vekthar/klasniecie") as AudioClip;
                SoundFXManager.Instance.PlaySoundFXClip(sandwitchAttack, transform,1f);
            } break;
            case 2: {
                _handLeft.Attack(VektharHand.HandState.Slam);
                AudioClip slamAttack = Resources.Load("SFX/vekthar/slap") as AudioClip;
                SoundFXManager.Instance.PlaySoundFXClip(slamAttack, transform,1f);
            } break;
        }

        _state = VektharState.DuringAttack;
    }

    private IEnumerator scheduleAttack(float secs) {
        yield return new WaitForSeconds(secs);
        Debug.Log("Scheduled attack's timer expired.");
        _state = VektharState.StartAttack;
    }
}