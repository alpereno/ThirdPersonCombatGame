using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    PlayerController playerController;
    public Vector2 moveInput;
    public Vector2 moveDirection;
    public float moveSpeed = 3.5f;
    public float maxSpeed = 7f;
    float desiredSpeed;
    float currentSpeed;

    const float groundAcceleration = 5;
    const float groundDeceleration = 25;

    Animator animator;

    bool IsMoveInput
    {
        get { return !Mathf.Approximately(moveInput.sqrMagnitude, 0f); }
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AssignMoveDirection(moveInput);
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void AssignMoveDirection(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude > 1f) moveDirection = moveInput.normalized;
        else moveDirection = moveInput;

        desiredSpeed = moveDirection.magnitude * maxSpeed;
        float acceleration = IsMoveInput ? groundAcceleration : groundDeceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);
        animator.SetFloat("CurrentSpeed", currentSpeed);

        moveDirection *= moveSpeed;

        playerController.SetVelocity(moveDirection);
    }
}
