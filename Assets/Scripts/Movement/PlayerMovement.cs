using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stamina Link")]
    public PlayerStamina staminaScript; // <-- Link to your stamina script

    [Header("Movement")]
    private float moveSpeed; // We'll get this from staminaScript now

    [Header("Ground Check")]
    public float playerHeight;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    // Sprint key removed from here because staminaScript handles it!
    public KeyCode sprintKey = KeyCode.LeftShift;

    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        MyInput();
        StateHandler();
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


    }

    private void StateHandler()
    {
        // Tell the movement script to sync its speed with whatever 
        // the Stamina script says we should be doing!
        if (staminaScript != null)
        {
            moveSpeed = staminaScript.currentMoveSpeed;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }




}