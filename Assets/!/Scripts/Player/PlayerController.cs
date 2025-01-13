using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(LivingEntity))]
public class PlayerController : MonoBehaviour
{
    // Movement
    public float Acceleration = 2f;
    public float Deceleration = 2f;
    public float MaxWalkingSpeed = 1f;
    public float MaxSprintingSpeed = 2f;
    public float MouseSensitivity = 0.1f;
    public float WalkingTurnSpeed = 1f;
    public float SprintingTurnSpeed = 2.5f;

    public GameObject CameraTargetObject;

    // State    
    private Vector2 _movementInputVector = Vector2.zero;
    private Vector3 _targetDirection;
    private bool _sprinting = false;
    private float _velocitySide = 0;
    private float _velocityFront = 0;
    public bool IsTurning { private get; set; }
    public bool ApplySpellTest = false;
    public bool ApplySpellTests = false;

    // References
    private CharacterController _controller;
    private Animator _animator;
    private LivingEntity _livingEntity;

    public LivingEntity LivingEntity { get => livingEntity; }
    
    // Animation IDs
    private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");
    private int _animationIdMoving = Animator.StringToHash("moving");
    private int _animationIdSprinting = Animator.StringToHash("sprinting");
    private int _animationIdJumping = Animator.StringToHash("jumping");

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _livingEntity = GetComponent<LivingEntity>();
        
        _targetDirection = transform.forward;
    }

    void Update()
    {
        if(ApplySpellTest) {
            new TestSpell().Cast(_livingEntity);
            ApplySpellTest = false;
        }

        if(ApplySpellTests) {
            new TestSpell().Cast(_livingEntity);
            ApplySpellTests = false;
        }

        // Make use of this once we have proper animations and model
        // HandleMovement();
        // HandleTurning();
        //
        // Rotate camera target to account for player mouse input 
        // cameraTargetObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
   
        // Placeholder for movement logic
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, _targetDirection);
     
        Vector3 movementVector = new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        movementVector = transform.TransformDirection(movementVector);

        _controller.SimpleMove(movementVector * 5);
    }

    // Handles animator movement logic
    private void handleMovement() {
        float currentMaxVelocity = _sprinting ? MaxSprintingSpeed : MaxWalkingSpeed;

        bool isVelocityXOverMax = _velocitySide > currentMaxVelocity || _velocitySide < -currentMaxVelocity;
        bool isVelocityYOverMax = _velocityFront > currentMaxVelocity || _velocityFront < -currentMaxVelocity;

        if(_movementInputVector.x != 0 && !isVelocityXOverMax) {
            _velocitySide += Acceleration * Time.deltaTime * _movementInputVector.x;
            if(_velocitySide > currentMaxVelocity) {
                _velocitySide = currentMaxVelocity;
            } else if(_velocitySide < -currentMaxVelocity) {
                _velocitySide = -currentMaxVelocity;
            }
        } else {
            if (_velocitySide > 0) {
                _velocitySide -= Deceleration * Time.deltaTime;
                if (_velocitySide < 0) {
                    _velocitySide = 0;
                }
            } else {
                _velocitySide += Deceleration * Time.deltaTime;
                if (_velocitySide > 0) {
                    _velocitySide = 0;
                }
            }
        }

        if(_movementInputVector.y != 0 && !isVelocityYOverMax) {
            _velocityFront += Acceleration * Time.deltaTime * _movementInputVector.y;
            if(_velocityFront > currentMaxVelocity) {
                _velocityFront = currentMaxVelocity;
            } else if(_velocityFront < -currentMaxVelocity) {
                _velocityFront = -currentMaxVelocity;
            } 
        } else {
            if (_velocityFront > 0) {
                _velocityFront -= Deceleration * Time.deltaTime;
                if (_velocityFront < 0) {
                    _velocityFront = 0;
                }
            } else {
                _velocityFront += Deceleration * Time.deltaTime;
                if (_velocityFront > 0) {
                    _velocityFront = 0;
                }
            }
        }

        _animator.SetFloat(_animationIdVelocitySide, _velocitySide);
        _animator.SetFloat(_animationIdVelocityFront, _velocityFront);


        if (_velocitySide != 0 || _velocityFront != 0) {
            _animator.SetBool("moving", true);
        } else {
            _animator.SetBool("moving", false);
        }
    }

    // Handles animator turning logic
    private void handleTurning() {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            float x = Vector3.SignedAngle(transform.forward, _targetDirection, Vector3.up);
            
            if(!IsTurning) {
                if(x < -135) {
                    _animator.SetTrigger("leftTurnFull");
                } else if(x > 135) {
                    _animator.SetTrigger("rightTurnFull");
                } else if(x > 45) {
                    _animator.SetTrigger("rightTurn");
                } else if(x < -45) {
                    _animator.SetTrigger("leftTurn");
                }
            }
        } else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Blend Tree")) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, _targetDirection), Time.deltaTime * WalkingTurnSpeed);
        } else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint Blend Tree")) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, _targetDirection), Time.deltaTime * SprintingTurnSpeed);
        }
    }

    void OnMove(InputValue value) {
        _movementInputVector = value.Get<Vector2>();
    }
    void OnLook(InputValue value) {
        Vector2 turnVector = value.Get<Vector2>();

        _targetDirection = Quaternion.Euler(0, turnVector.x * MouseSensitivity, 0) * _targetDirection;
    }

    void OnSprint(InputValue value) {
        _sprinting = value.isPressed;
        _animator.SetBool("sprinting", _sprinting);
    }

    void OnAttack(InputValue value) {
        _animator.SetTrigger("punch");
    }

    void OnJump(InputValue value) {
        _animator.SetTrigger("jump");
    }

    // Animation events
    void OnFootstep() {
        Debug.Log("Footstep");
    }

    void OnLand() {
        Debug.Log("Land");
    }

    public void OnDeath() {
        Debug.Log("Player died");
    }
}
