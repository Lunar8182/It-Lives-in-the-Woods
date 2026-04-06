using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stamina Link")]
    public PlayerStamina staminaScript;

    [Header("Movement Settings")]
    private float moveSpeed;
    [Tooltip("How fast the player stops when releasing the keys (Higher = snappier)")]
    public float brakeSpeed = 15f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public float groundDrag = 5f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    private bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;

    // Input Tracking
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        StateHandler();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && grounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        if (staminaScript != null)
        {
            moveSpeed = staminaScript.currentMoveSpeed;
        }
        else
        {
            moveSpeed = 5f;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        if (grounded)
        {
            if (isMoving)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else
            {

                Vector3 currentVelocity = rb.linearVelocity;
                rb.linearVelocity = new Vector3(
                    Mathf.Lerp(currentVelocity.x, 0f, brakeSpeed * Time.fixedDeltaTime),
                    currentVelocity.y, // Leave gravity alone!
                    Mathf.Lerp(currentVelocity.z, 0f, brakeSpeed * Time.fixedDeltaTime)
                );
            }
        }
        else
        {
            if (isMoving)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
    }

    private void SpeedControl()
    {
        // Get the player's horizontal velocity
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}