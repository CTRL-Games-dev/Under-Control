using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class VektharBoss : MonoBehaviour {
    enum VektharState {
        WakeUp,
        Follow,
        StartAttack,
        DuringAttack,
        EndAttack,
        
        StartHurt,
        DuringHurt,
        EndHurt,
    }

    public float Damage = 40;

    private VektharState _state = VektharState.WakeUp;

    [SerializeField] private GameObject _eyes;
    [SerializeField] private GameObject _body;
    [SerializeField] private VektharHand _handLeft;
    [SerializeField] private VektharHand _handRight;
    
    public bool attacking = false;
    private bool _wasPlayerHit = false;

    public LivingEntity LivingEntity { get; private set; }

    void Awake() {
        LivingEntity = GetComponent<LivingEntity>();
    }

    void Start() {
        _handLeft.TargetHit.AddListener(hitPlayer);
        _handRight.TargetHit.AddListener(hitPlayer);
    }

    private void hitPlayer(LivingEntity entity) {
        if(entity == LivingEntity) return;
        if(!attacking) return;
        if(_wasPlayerHit) return;

        LivingEntity.Attack(new Damage {
            Type = DamageType.PHYSICAL,
            Value = Damage
        }, entity);

        _wasPlayerHit = true;
    }

    void Update() {
        switch (_state) {
            case VektharState.WakeUp: {
                _state = VektharState.Follow;
                StartCoroutine(scheduleAttack(5));
                break;
            }
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
                break;
            }
            case VektharState.EndAttack: {
                StartCoroutine(scheduleAttack(3));
                _state = VektharState.Follow;
                break;
            }
            case VektharState.StartHurt: {
                _state = VektharState.DuringHurt;
                break;
            }
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

        _wasPlayerHit = false;

        int attackNumber = UnityEngine.Random.Range(0, 3);
        switch (attackNumber) {
            case 0:
                _handRight.Attack(VektharHand.HandState.Fist);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.Fist, this.transform.position);
            break;
            case 1:
                _handLeft.Attack(VektharHand.HandState.Sandwitch);
                _handRight.Attack(VektharHand.HandState.Sandwitch);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.Clap, this.transform.position);
            break;
            case 2: 
                _handLeft.Attack(VektharHand.HandState.Slam);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.OpenHand, this.transform.position);
            break;
        }

        _state = VektharState.DuringAttack;
    }

    private IEnumerator scheduleAttack(float secs) {
        yield return new WaitForSeconds(secs);
        Debug.Log("Scheduled attack's timer expired.");
        _state = VektharState.StartAttack;
    }
}