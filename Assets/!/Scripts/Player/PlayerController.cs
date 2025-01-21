using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(LivingEntity))]
public class PlayerController : MonoBehaviour
{
    // UnityEvents
    public UnityEvent OnInventoryToggleEvent;
    public UnityEvent OnUICancelEvent;

    // Movement
    public float Acceleration = 2f;
    public float Deceleration = 2f;
    public float MaxWalkingSpeed = 1f;
    public float MaxSprintingSpeed = 2f;
    public float MouseSensitivity = 0.1f;
    public float WalkingTurnSpeed = 1f;
    // public float SprintingTurnSpeed = 2.5f;
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public DynamicStat VekhtarControl = new DynamicStat(StatType.VEKTHAR_CONTROL, 0);    
    public GameObject CameraObject;
    public GameObject CameraTargetObject;

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    // private Vector3 _targetDirection;
    // private bool _sprinting = false;
    // private float _velocitySide = 0;
    // private float _velocityFront = 0;
    // private bool _isTurning;
    private float _cameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public LivingEntity LivingEntity { get; private set; }
    public CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    
    // Animation IDs
    private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");
    private int _animationIdMoving = Animator.StringToHash("moving");
    private int _animationIdSprinting = Animator.StringToHash("sprinting");
    private int _animationIdJumping = Animator.StringToHash("jumping");

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        LivingEntity = GetComponent<LivingEntity>();
        CinemachinePositionComposer = CameraObject.GetComponent<CinemachinePositionComposer>();
        
        // _targetDirection = transform.forward;
        _cameraDistance = MinCameraDistance;
    }

    void Update()
    {
        recalculateStats();

        // if(ApplySpellTest) {
        //     new TestSpell().Cast(_livingEntity);
        //     ApplySpellTest = false;
        // }

        // if(ApplySpellTests) {
        //     new TestSpell().Cast(_livingEntity);
        //     ApplySpellTests = false;
        // }

        // Make use of this once we have proper animations and model
        // HandleMovement();
        // HandleTurning();
        //
        // Rotate camera target to account for player mouse input 
        // cameraTargetObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
   
        // Placeholder for movement logic
        // I guess in our game we don't need this?
        // transform.rotation = Quaternion.FromToRotation(Vector3.forward, _targetDirection);
     
        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);

        CharacterController.SimpleMove(movementVector * 5);
    }

    // Handles animator movement logic
    // private void handleMovement() {
    //     float currentMaxVelocity = _sprinting ? MaxSprintingSpeed : MaxWalkingSpeed;

    //     bool isVelocityXOverMax = _velocitySide > currentMaxVelocity || _velocitySide < -currentMaxVelocity;
    //     bool isVelocityYOverMax = _velocityFront > currentMaxVelocity || _velocityFront < -currentMaxVelocity;

    //     if(_movementInputVector.x != 0 && !isVelocityXOverMax) {
    //         _velocitySide += Acceleration * Time.deltaTime * _movementInputVector.x;
    //         if(_velocitySide > currentMaxVelocity) {
    //             _velocitySide = currentMaxVelocity;
    //         } else if(_velocitySide < -currentMaxVelocity) {
    //             _velocitySide = -currentMaxVelocity;
    //         }
    //     } else {
    //         if (_velocitySide > 0) {
    //             _velocitySide -= Deceleration * Time.deltaTime;
    //             if (_velocitySide < 0) {
    //                 _velocitySide = 0;
    //             }
    //         } else {
    //             _velocitySide += Deceleration * Time.deltaTime;
    //             if (_velocitySide > 0) {
    //                 _velocitySide = 0;
    //             }
    //         }
    //     }

    //     if(_movementInputVector.y != 0 && !isVelocityYOverMax) {
    //         _velocityFront += Acceleration * Time.deltaTime * _movementInputVector.y;
    //         if(_velocityFront > currentMaxVelocity) {
    //             _velocityFront = currentMaxVelocity;
    //         } else if(_velocityFront < -currentMaxVelocity) {
    //             _velocityFront = -currentMaxVelocity;
    //         } 
    //     } else {
    //         if (_velocityFront > 0) {
    //             _velocityFront -= Deceleration * Time.deltaTime;
    //             if (_velocityFront < 0) {
    //                 _velocityFront = 0;
    //             }
    //         } else {
    //             _velocityFront += Deceleration * Time.deltaTime;
    //             if (_velocityFront > 0) {
    //                 _velocityFront = 0;
    //             }
    //         }
    //     }

    //     _animator.SetFloat(_animationIdVelocitySide, _velocitySide);
    //     _animator.SetFloat(_animationIdVelocityFront, _velocityFront);


    //     if (_velocitySide != 0 || _velocityFront != 0) {
    //         _animator.SetBool("moving", true);
    //     } else {
    //         _animator.SetBool("moving", false);
    //     }
    // }

    // // Handles animator turning logic
    // private void handleTurning() {
    //     if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
    //         float x = Vector3.SignedAngle(transform.forward, _targetDirection, Vector3.up);
            
    //         if(!_isTurning) {
    //             if(x < -135) {
    //                 _animator.SetTrigger("leftTurnFull");
    //             } else if(x > 135) {
    //                 _animator.SetTrigger("rightTurnFull");
    //             } else if(x > 45) {
    //                 _animator.SetTrigger("rightTurn");
    //             } else if(x < -45) {
    //                 _animator.SetTrigger("leftTurn");
    //             }
    //         }
    //     } else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Blend Tree")) {
    //         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, _targetDirection), Time.deltaTime * WalkingTurnSpeed);
    //     } else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint Blend Tree")) {
    //         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, _targetDirection), Time.deltaTime * SprintingTurnSpeed);
    //     }
    // }

    private void recalculateStats() {
        VekhtarControl.Recalculate(LivingEntity.ModifierSystem);
    }

    // Input events

    void OnMove(InputValue value) {
        _movementInputVector = value.Get<Vector2>();
    }
    void OnLook(InputValue value) {
        // Vector2 turnVector = value.Get<Vector2>();

        // _targetDirection = Quaternion.Euler(0, turnVector.x * MouseSensitivity, 0) * _targetDirection;

        Vector2 pointerVector = value.Get<Vector2>();
        pointerVector /= new Vector2(Screen.width, Screen.height);
        if (pointerVector.x < 0) {
            pointerVector.x = 0;
        }
        if (pointerVector.x > 1) {
            pointerVector.x = 1;
        }
        if (pointerVector.y < 0) {
            pointerVector.y = 0;
        }
        if (pointerVector.y > 1) {
            pointerVector.y = 1;
        }

        pointerVector *= 2;
        pointerVector -= Vector2.one;

        var pointerVectorPos = pointerVector * CameraTargetObjectBounds;

        CameraTargetObject.transform.localPosition = new Vector3(
            pointerVectorPos.x,
            CameraTargetObject.transform.localPosition.y,
            pointerVectorPos.y
        );
    }

    // void OnSprint(InputValue value) {
    //     _sprinting = value.isPressed;
    //     _animator.SetBool("sprinting", _sprinting);
    // }

    // void OnAttack(InputValue value) {
    //     _animator.SetTrigger("punch");
    // }

    // void OnJump(InputValue value) {
    //     _animator.SetTrigger("jump");
    // }

    void OnScrollWheel(InputValue value) {
        var delta = value.Get<Vector2>();
        _cameraDistance -= delta.y * CameraDistanceSpeed;
        if (_cameraDistance < MinCameraDistance) {
            _cameraDistance = MinCameraDistance;
        }
        if (_cameraDistance > MaxCameraDistance) {
            _cameraDistance = MaxCameraDistance;
        }
    }

    void OnToggleInventory(InputValue value) {
        OnInventoryToggleEvent.Invoke();
    }

    void OnCancel(InputValue value) {
        OnUICancelEvent.Invoke();
    }

    // Animation events

    void OnFootstep() {
        Debug.Log("Footstep");
    }

    void OnLand() {
        Debug.Log("Land");
    }

    void OnPlayerDeath() {
        Debug.Log("Player died");
    }
}
