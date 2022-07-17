using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    // handle functionality to move player on this script

    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;
    Rigidbody playerRb;

    Vector3 moveDirection;
    Transform cameraObject;

    [Header("Falling")]
    [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingVelocity;
    [SerializeField] private float fallingVelocity;
    [SerializeField] private float rayCastHeightOffSet = .5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    [SerializeField] private bool isGrounded;

    [Header ("Movement Speeds")]
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float sprintingSpeed = 7;
    [SerializeField] private float rotationSpeed = 15;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRb = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)        
            return;
        
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection = moveDirection.normalized;
        moveDirection.y = 0;
        Vector3 moveVelocity;
        // if we are sprinting, select the sprinting speed
        // if we are running, select the running speed
        // if we are walking, select the walking speed

        if (isSprinting)
        {
            moveVelocity = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= .5f)
            {
                moveVelocity = moveDirection * runningSpeed;
            }
            else
            {
                moveVelocity = moveDirection * walkingSpeed;
            }
        }

        //playerRb.MovePosition(playerRb.position + moveVelocity);
        playerRb.velocity = moveVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection = targetDirection.normalized;
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        RaycastHit hit;
        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRb.AddForce(transform.forward * leapingVelocity);
            playerRb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }
        if (Physics.SphereCast(rayCastOrigin,.2f, Vector3.down, out hit, .5f,groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
