using DG.Tweening;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] 
    float linearAcceleration = 10f;
    [SerializeField]
    float maxWalkSpeed = 2f;
    [SerializeField]
    float maxRunSpeed = 5f;
    [SerializeField]
    float decelerationFactor = 10f;

    Vector3 rawStickValue;
    Vector3 projectedMovement;
    Vector3 velocityOnPlane = Vector3.zero;

    [Header("Jump")]
    [SerializeField]
    float jumpSpeed = 5f;
    bool mustJump = false;
    float verticalVelocity = 0f;

    const float gravity = -9.8f;

    [Header("Walk")]
    bool isWalking = false;

    public enum OrientationMode
    {
        MovementDirection,
        CameraDirection,
        FaceToTarget
    };
    [Header("Oritentation")]
    [SerializeField]
    OrientationMode orientationMode = OrientationMode.MovementDirection;
    [SerializeField]
    float angularVelocity = 360f;
    [SerializeField]
    Transform target;

    Vector3 lastMovementDirection = Vector3.zero;

    [Header("Combat")]
    [SerializeField]
    Transform hitCollidersParent;

    [Header("Inputs Movement")]
    [SerializeField]
    InputActionReference move;
    [SerializeField]
    InputActionReference jump;
    [SerializeField]
    InputActionReference walk;

    Animator animator;
    CharacterController characterController;
    Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        walk.action.Enable();

        move.action.started += OnMove;
        move.action.performed += OnMove;
        move.action.canceled += OnMove;

        jump.action.performed += OnJump;

        walk.action.started += OnWalk;
        walk.action.canceled += OnWalk;

        foreach (AnimationEventForwarder aef in GetComponentsInChildren<AnimationEventForwarder>())
        {
            aef.onAnimationEvent.AddListener(OnAnimationEvent);
        }
    }

    void Update()
    {
        Vector3 compositeMovement = Vector3.zero;

        compositeMovement += UpdateMovementOnPlane();
        compositeMovement += UpdateVerticalMovement();

        characterController.Move(compositeMovement);

        UpdateOrientation();
        UpdateAnimation();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        walk.action.Disable();

        move.action.started -= OnMove;
        move.action.performed -= OnMove;
        move.action.canceled -= OnMove;

        jump.action.performed -= OnJump;

        walk.action.started -= OnWalk;
        walk.action.canceled -= OnWalk;

        foreach (AnimationEventForwarder aef in GetComponentsInChildren<AnimationEventForwarder>())
        {
            aef.onAnimationEvent.RemoveListener(OnAnimationEvent);
        }
    }

    #region Input Events
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 stickValue = context.ReadValue<Vector2>();

        rawStickValue = (Vector3.forward * stickValue.y) + (Vector3.right * stickValue.x);  
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        mustJump = true;
    }

    private void OnWalk(InputAction.CallbackContext context)
    {
        isWalking = context.ReadValueAsButton();
        
    }
    #endregion

    void OnAnimationEvent(string hitColliderName)
    {
        hitCollidersParent.Find(hitColliderName)?.gameObject.SetActive(true);
    }

    private Vector3 UpdateMovementOnPlane()
    {
        // Deceleration
        //if (rawStickValue.magnitude <= 0.01f)
        {
            Vector3 decelerationOnPlane = -velocityOnPlane * decelerationFactor * Time.deltaTime;
            velocityOnPlane += decelerationOnPlane;
        }

        // Acceleration
        Vector3 acceleration = (mainCamera.transform.forward * rawStickValue.z) + (mainCamera.transform.right * rawStickValue.x);
        float accelerationLength = acceleration.magnitude;
        Vector3 projectedAcceleration = Vector3.ProjectOnPlane(acceleration, Vector3.up).normalized * accelerationLength;
        Vector3 deltaAccelerationOnPlane = projectedAcceleration * linearAcceleration * Time.deltaTime;

        // Max speed
        float maxSpeed = CalcMaxSpeed();

        float currentSpeed = velocityOnPlane.magnitude;
        float attainableVelocity = Mathf.Max(currentSpeed, maxSpeed);
        velocityOnPlane += deltaAccelerationOnPlane;
        velocityOnPlane = Vector3.ClampMagnitude(velocityOnPlane, attainableVelocity);

        return velocityOnPlane * Time.deltaTime;
    }

    private float CalcMaxSpeed()
    {
        return isWalking ? maxWalkSpeed : maxRunSpeed;
    }

    private Vector3 UpdateVerticalMovement()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }

        if (mustJump)
        {
            mustJump = false;
            if (characterController.isGrounded)
            {
                verticalVelocity = jumpSpeed;
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        return Vector3.up * verticalVelocity * Time.deltaTime;
    }

    private void UpdateOrientation()
    {
        Vector3 desiredDirection = CalculateDesiredDirection();

        RotateToDesiredDirection(desiredDirection);

        Vector3 CalculateDesiredDirection()
        {
            Vector3 desiredDirection = Vector3.zero;
            switch (orientationMode)
            {
                case OrientationMode.MovementDirection:
                    if (rawStickValue.magnitude < 0.01f)
                    {
                        desiredDirection = lastMovementDirection;
                    }
                    else
                    {
                        desiredDirection = velocityOnPlane;
                        lastMovementDirection = desiredDirection;
                    }
                    break;
                case OrientationMode.CameraDirection:
                    desiredDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
                    break;
                case OrientationMode.FaceToTarget:
                    desiredDirection = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);
                    break;
            }

            return desiredDirection;
        }

        void RotateToDesiredDirection(Vector3 desiredDirection)
        {
            float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float angleToApply = angularVelocity * Time.deltaTime;
            angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));

            Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply * Mathf.Sign(angularDistance), Vector3.up);
            transform.rotation = rotationToApply * transform.rotation;
        }
    }

    private void UpdateAnimation()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocityOnPlane);
        float maxSpeed = maxRunSpeed;
        animator.SetFloat("HorizontalVelocity", localVelocity.x / maxSpeed);
        animator.SetFloat("ForwardVelocity", localVelocity.z / maxSpeed);

        float jumpProgress = Mathf.InverseLerp(jumpSpeed, -jumpSpeed, verticalVelocity);
        
        animator.SetFloat("JumpProgress", characterController.isGrounded ? 1f : jumpProgress);
        animator.SetBool("IsGrounded", characterController.isGrounded);
    }
}
