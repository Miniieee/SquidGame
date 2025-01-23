using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float GravityValue = -9.81f;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float sprintSpeed;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        inputManager = InputManager.Instance;
        if (Camera.main != null) cameraTransform = Camera.main.transform;

        controller = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }

        Vector2 input = inputManager.GetPlayerMovement();
        
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * input.y + cameraRight * input.x;

        moveDirection.Normalize();

        float currentSpeed = inputManager.GetSprint() ? sprintSpeed : playerSpeed;

        // Move the player
        controller.Move(moveDirection * (currentSpeed * Time.deltaTime));

        // Apply gravity
        playerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
