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
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxWalkingSpeed = 1f;
    public float maxSprintingSpeed = 2f;
    public float mouseSensitivity = 0.1f;
    public float walkingTurnSpeed = 1f;
    public float sprintingTurnSpeed = 2.5f;

    public GameObject cameraTargetObject;

    // State    
    private Vector2 movementInputVector = Vector2.zero;
    private Vector3 targetDirection;
    private bool sprinting = false;
    private float velocitySide = 0;
    private float velocityFront = 0;
    public bool isTurning { private get; set; }
    public bool applySpellTest = false;
    public bool applySpellTests = false;

    // References
    private CharacterController controller;
    private Animator animator;
    private LivingEntity livingEntity;
    
    // Animation IDs
    private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");
    private int _animationIdMoving = Animator.StringToHash("moving");
    private int _animationIdSprinting = Animator.StringToHash("sprinting");
    private int _animationIdJumping = Animator.StringToHash("jumping");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        livingEntity = GetComponent<LivingEntity>();
        
        targetDirection = transform.forward;
    }

    void Update()
    {
        if(applySpellTest) {
            new TestSpell().Cast(livingEntity);
            applySpellTest = false;
        }

        if(applySpellTests) {
            new TestSpell().Cast(livingEntity);
            applySpellTests = false;
        }

        // Make use of this once we have proper animations and model
        // HandleMovement();
        // HandleTurning();
        //
        // Rotate camera target to account for player mouse input 
        // cameraTargetObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
   
        // Placeholder for movement logic
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
     
        Vector3 movementVector = new Vector3(movementInputVector.x, 0, movementInputVector.y);
        movementVector = transform.TransformDirection(movementVector);

        controller.SimpleMove(movementVector * 5);
    }

    // Handles animator movement logic
    private void HandleMovement() {
        float currentMaxVelocity = sprinting ? maxSprintingSpeed : maxWalkingSpeed;

        bool isVelocityXOverMax = velocitySide > currentMaxVelocity || velocitySide < -currentMaxVelocity;
        bool isVelocityYOverMax = velocityFront > currentMaxVelocity || velocityFront < -currentMaxVelocity;

        if(movementInputVector.x != 0 && !isVelocityXOverMax) {
            velocitySide += acceleration * Time.deltaTime * movementInputVector.x;
            if(velocitySide > currentMaxVelocity) {
                velocitySide = currentMaxVelocity;
            } else if(velocitySide < -currentMaxVelocity) {
                velocitySide = -currentMaxVelocity;
            }
        } else {
            if (velocitySide > 0) {
                velocitySide -= deceleration * Time.deltaTime;
                if (velocitySide < 0) {
                    velocitySide = 0;
                }
            } else {
                velocitySide += deceleration * Time.deltaTime;
                if (velocitySide > 0) {
                    velocitySide = 0;
                }
            }
        }

        if(movementInputVector.y != 0 && !isVelocityYOverMax) {
            velocityFront += acceleration * Time.deltaTime * movementInputVector.y;
            if(velocityFront > currentMaxVelocity) {
                velocityFront = currentMaxVelocity;
            } else if(velocityFront < -currentMaxVelocity) {
                velocityFront = -currentMaxVelocity;
            } 
        } else {
            if (velocityFront > 0) {
                velocityFront -= deceleration * Time.deltaTime;
                if (velocityFront < 0) {
                    velocityFront = 0;
                }
            } else {
                velocityFront += deceleration * Time.deltaTime;
                if (velocityFront > 0) {
                    velocityFront = 0;
                }
            }
        }

        animator.SetFloat(_animationIdVelocitySide, velocitySide);
        animator.SetFloat(_animationIdVelocityFront, velocityFront);


        if (velocitySide != 0 || velocityFront != 0) {
            animator.SetBool("moving", true);
        } else {
            animator.SetBool("moving", false);
        }
    }

    // Handles animator turning logic
    private void HandleTurning() {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            float x = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
            
            if(!isTurning) {
                if(x < -135) {
                    animator.SetTrigger("leftTurnFull");
                } else if(x > 135) {
                    animator.SetTrigger("rightTurnFull");
                } else if(x > 45) {
                    animator.SetTrigger("rightTurn");
                } else if(x < -45) {
                    animator.SetTrigger("leftTurn");
                }
            }
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Blend Tree")) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, targetDirection), Time.deltaTime * walkingTurnSpeed);
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint Blend Tree")) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, targetDirection), Time.deltaTime * sprintingTurnSpeed);
        }
    }

    void OnMove(InputValue value) {
        movementInputVector = value.Get<Vector2>();
    }
    void OnLook(InputValue value) {
        Vector2 turnVector = value.Get<Vector2>();

        targetDirection = Quaternion.Euler(0, turnVector.x * mouseSensitivity, 0) * targetDirection;
    }

    void OnSprint(InputValue value) {
        sprinting = value.isPressed;
        animator.SetBool("sprinting", sprinting);
    }

    void OnAttack(InputValue value) {
        animator.SetTrigger("punch");
    }

    void OnJump(InputValue value) {
        animator.SetTrigger("jump");
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
