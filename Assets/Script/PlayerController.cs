using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        ApplyGravity();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
            speed = runSpeed;

        if (isCrouching)
            speed = crouchSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // keeps player grounded
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}