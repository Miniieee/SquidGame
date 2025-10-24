using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private Transform cameraTransform;
    
    private InputActions inputActions;
    private Vector3 currentMovement;
    private Vector2 currentmovementInput;
    private Rigidbody rigidBody;
    private Vector2 currentMovementInput;
    
    private void Awake()
    {
        inputActions = new InputActions();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    private void Update()
    {
        currentMovementInput = inputActions.Player.Move.ReadValue<Vector2>();
        
        if (!cameraTransform)
        {
            currentMovement = new Vector3(currentMovementInput.x, 0f, currentMovementInput.y);
        }
        else
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();
            currentMovement = camForward * currentMovementInput.y + camRight * currentMovementInput.x;
        }
    }

    private void FixedUpdate()
    {
        //could be currentMovement.magnitude but less efficient
        if (currentMovement != Vector3.zero) 
        {
            rigidBody.AddForce(currentMovement.normalized * acceleration, ForceMode.Acceleration);
            
            if (rigidBody.linearVelocity.magnitude > maxSpeed)
            {
                rigidBody.linearVelocity = rigidBody.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            rigidBody.AddForce(- rigidBody.linearVelocity.normalized * deceleration, ForceMode.Acceleration);
        }
    }
}
