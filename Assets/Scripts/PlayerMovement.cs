using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private InputManager inputManager;
    private Transform cameraTransform;

    // We'll store our velocity in a Vector3. 
    // Y component will be affected by gravity.
    private Vector3 playerVelocity;
    private bool isGrounded;

    private void Start()
    {
        // Get references
        inputManager = InputManager.Instance;
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        
        // Lock rotation so the player doesn't tip over
        rb.freezeRotation = true;
        
        // We can manually handle gravity by turning off built-in gravity:
        rb.useGravity = false;

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // We’ll handle input in Update (since it’s more responsive),
        // but actually move the player in FixedUpdate for physics consistency.
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        // 1) Check if we’re grounded
        //    We can raycast straight down from the player's center to detect ground.
        float capsuleHalfHeight = capsule.height * 0.5f;
        Vector3 rayStart = transform.position;  // center of the player

        // If the ray (cast straight down) hits something within capsuleHalfHeight + groundCheckDistance
        // we consider ourselves grounded.
        isGrounded = Physics.Raycast(rayStart, Vector3.down, 
                                     capsuleHalfHeight + groundCheckDistance);

        // 2) If grounded, reset downward velocity so we don’t accumulate negative y forever
        if (isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = 0f;
        }

        // 3) Get the input from your InputManager
        Vector2 input = inputManager.GetPlayerMovement();

        // 4) Determine direction relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten the directions so we don’t tilt up/down when looking up/down
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Combine input
        Vector3 moveDirection = forward * input.y + right * input.x;
        moveDirection.Normalize();

        // 5) Decide whether we’re sprinting or walking
        float currentSpeed = inputManager.GetSprint() ? sprintSpeed : walkSpeed;

        // 6) Update horizontal velocity
        playerVelocity.x = moveDirection.x * currentSpeed;
        playerVelocity.z = moveDirection.z * currentSpeed;

        // 7) Apply gravity manually
        //    (We turned off useGravity, so we handle it ourselves.)
        playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;

        // 8) Assign final velocity to the Rigidbody
        rb.linearVelocity = playerVelocity;
    }
}
