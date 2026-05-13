using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaDrain = 1f;
    public float staminaRecovery = 0.8f;
    public Image staminaBarFill;

    private float currentStamina;
    [Header("Stamina UI Fade")]
    public float staminaFadeSpeed = 5f;
    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip defaultWalkSound;
    public AudioClip grassWalkSound;
    public AudioClip jumpSound;

    [Header("Head Bob")]
    public Transform cameraHolder;

    public float walkBobSpeed = 6f;
    public float walkBobAmount = 0.05f;

    public float sprintBobSpeed = 10f;
    public float sprintBobAmount = 0.1f;

    private float defaultCameraY;
    private float bobTimer;
    private float staminaTargetAlpha = 0f;
    private CharacterController controller;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;

        currentStamina = maxStamina;

        if (cameraHolder != null)
            defaultCameraY = cameraHolder.localPosition.y;

        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = 1f;

            Color c = staminaBarFill.color;
            c.a = 0f;
            staminaBarFill.color = c;
        }
    }
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        ApplyGravity();
        HandleFootsteps();
        HandleHeadBob();
        HandleStaminaUI();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool moving = x != 0 || z != 0;

        float speed = walkSpeed;

        isRunning = false;

        if (Input.GetKey(KeyCode.LeftShift) &&
            !isCrouching &&
            currentStamina > 0 &&
            moving)
        {
            speed = runSpeed;
            isRunning = true;

            currentStamina -= staminaDrain * Time.deltaTime;
        }
        if (!isRunning)
        {
            currentStamina += staminaRecovery * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (isCrouching)
            speed = crouchSpeed;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            Time.deltaTime * crouchTransitionSpeed
        );
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootsteps()
    {
        if (!isGrounded)
        {
            StopFootsteps();
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        bool isMoving = x != 0 || z != 0;

        if (!isMoving)
        {
            StopFootsteps();
            return;
        }

        AudioClip targetClip = GetGroundFootstepSound();

        if (audioSource.clip != targetClip)
        {
            audioSource.Stop();
            audioSource.clip = targetClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (!audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopFootsteps()
    {
        if (audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Stop();
        }
    }

    AudioClip GetGroundFootstepSound()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
        {
            if (hit.collider.CompareTag("Grass"))
            {
                return grassWalkSound;
            }
        }

        return defaultWalkSound;
    }

    void HandleHeadBob()
    {
        if (cameraHolder == null) return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        bool isMoving = x != 0 || z != 0;

        if (!isGrounded || !isMoving)
        {
            Vector3 resetPos = cameraHolder.localPosition;

            resetPos.y = Mathf.Lerp(
                resetPos.y,
                defaultCameraY,
                Time.deltaTime * 8f
            );

            cameraHolder.localPosition = resetPos;
            return;
        }

        float bobSpeed = isRunning ? sprintBobSpeed : walkBobSpeed;
        float bobAmount = isRunning ? sprintBobAmount : walkBobAmount;

        bobTimer += Time.deltaTime * bobSpeed;

        Vector3 pos = cameraHolder.localPosition;
        pos.y = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount;

        cameraHolder.localPosition = pos;
    }

    void HandleStaminaUI()
    {
        if (staminaBarFill == null) return;

        staminaBarFill.fillAmount =
            Mathf.Clamp01(currentStamina / maxStamina);

        if (isRunning)
        {
            staminaTargetAlpha = 1f;
        }
        else
        {
            staminaTargetAlpha = 0f;
        }

        Color c = staminaBarFill.color;

        c.a = Mathf.Lerp(
            c.a,
            staminaTargetAlpha,
            Time.deltaTime * staminaFadeSpeed
        );

        staminaBarFill.color = c;
    }
}