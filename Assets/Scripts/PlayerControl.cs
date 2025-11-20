using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;

    private float movementX;
    private float movementY;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 targetDirection = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        InputSystem.actions.Disable();
        playerInput.currentActionMap?.Enable();
    }

    // Called from Input System when stick / WASD changes
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;

        // Build the direction vector
        targetDirection = new Vector3(movementX, 0f, movementY);
    }

    private void FixedUpdate()
    {
        // No input = no rotation or movement
        if (targetDirection == Vector3.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        // ---- 1. ROTATE FIRST ----
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        // Check if robot faces the target direction
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // ---- 2. MOVE ONLY WHEN FACING THE RIGHT DIRECTION ----
        if (angle < 5f) // tolerance angle
        {
            Vector3 forwardMove = transform.forward * moveSpeed;
            rb.linearVelocity = new Vector3(forwardMove.x, rb.linearVelocity.y, forwardMove.z);
        }
        else
        {
            // While turning, don't move forward
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}
