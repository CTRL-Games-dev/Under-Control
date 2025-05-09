using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class TreeBossController : MonoBehaviour {
    public Pinecone PineconePrefab;
    public GameObject PineconeSpawner;

    public RootAttack RootAttack1;
    public RootAttack RootAttack2;
    public RootAttack RootAttack3;

    private LivingEntity _livingEntity;
    private BehaviorGraphAgent _behaviorGraphAgent;
    private Vector3 _targetStartingPosition;
    private float _targetStartingPositionTime;
    private LivingEntity _target;

    public void Awake() {
        _livingEntity = GetComponent<LivingEntity>();
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
    }

    public void Start()
    {
        CameraManager.SwitchCamera(Player.Instance.TopDownCamera);
    }

    public void PrepareThrowAttack() {
        if(!_behaviorGraphAgent.BlackboardReference.GetVariable("Target", out BlackboardVariable<LivingEntity> target)) return;
        if(target.Value == null) return;
        
        LivingEntity targetEntity = target.Value;

        _targetStartingPositionTime = Time.time;
        _targetStartingPosition = targetEntity.transform.position;
        _target = targetEntity;
    }

    public void PrepareRootAttack() {
        if(!_behaviorGraphAgent.BlackboardReference.GetVariable("Target", out BlackboardVariable<LivingEntity> target)) return;
        if(target.Value == null) return;
        
        LivingEntity targetEntity = target.Value;

        _targetStartingPositionTime = Time.time;
        _targetStartingPosition = targetEntity.transform.position;
        _target = targetEntity;   
    }

    public void OnPineconeThrow() {
        CameraManager.ShakeCamera(1, 0.5f);
      
        Vector3 targetSpeed = (_target.transform.position - _targetStartingPosition) / (Time.time - _targetStartingPositionTime);

        Vector3 predictedPlayerPosition = calculateInterceptPoint(
            PineconeSpawner.transform.position,
            _target.transform.position + Vector3.up,
            targetSpeed,
            20
        );

        Vector3 direction = (predictedPlayerPosition - PineconeSpawner.transform.position).normalized;

        Pinecone pinecone = Instantiate(PineconePrefab, PineconeSpawner.transform.position, Quaternion.LookRotation(direction));
        pinecone.Initialize(_livingEntity, direction);

        _targetStartingPosition = Vector3.zero;
    }

    public void OnRootAttack() {
        CameraManager.ShakeCamera(10, 1);
   
        Vector3 targetSpeed = (_target.transform.position - _targetStartingPosition) / (Time.time - _targetStartingPositionTime);

        Vector3 predictedPlayerPosition = calculateInterceptPoint(
            RootAttack1.transform.position,
            _target.transform.position,
            targetSpeed,
            20
        );

        Vector3 direction = (predictedPlayerPosition - RootAttack1.transform.position).normalized;

        RootAttack1.transform.rotation = Quaternion.LookRotation(direction);
        RootAttack1.Attack();

        RootAttack2.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, Random.Range(15, 75), 0);
        RootAttack2.Attack();

        RootAttack3.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, Random.Range(-15, -75), 0);
        RootAttack3.Attack();
    }

    private Vector3 calculateInterceptPoint(
        Vector3 shooterPos,
        Vector3 targetPos,
        Vector3 targetVelocity,
        float projectileSpeed)
    {
        Vector3 D = targetPos - shooterPos;
        float a = Vector3.Dot(targetVelocity, targetVelocity) - projectileSpeed * projectileSpeed;
        float b = 2f * Vector3.Dot(D, targetVelocity);
        float c = Vector3.Dot(D, D);
        float disc = b * b - 4f * a * c;
        if (disc < 0f) return targetPos; // no solution, aim at current position

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);
        float t = Mathf.Min(t1 > 0f ? t1 : float.MaxValue,
                            t2 > 0f ? t2 : float.MaxValue);
        if (t == float.MaxValue) return targetPos; // both times negative

        return targetPos + targetVelocity * t;
    }
}