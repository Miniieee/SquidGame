using UnityEngine;
using Unity.Cinemachine;  // Make sure you have using Cinemachine for CinemachineVirtualCamera
using System.Collections.Generic;

public class PlayerScan : MonoBehaviour
{
    private GameObject[] players;

    // We'll store the last known position and rotation of each player's Cinemachine camera
    private Dictionary<GameObject, Vector3> lastCameraPositions;
    private Dictionary<GameObject, Quaternion> lastCameraRotations;

    [SerializeField] private float positionThreshold = 0.1f;  
    [SerializeField] private float rotationThreshold = 5f; // in degrees

    void Start()
    {
        // Find all GameObjects tagged "Player"
        players = GameObject.FindGameObjectsWithTag("Player");

        lastCameraPositions = new Dictionary<GameObject, Vector3>();
        lastCameraRotations = new Dictionary<GameObject, Quaternion>();

        // For each player, find the CinemachineVirtualCamera child
        foreach (GameObject player in players)
        {
            CinemachineCamera vcam = player.GetComponentInChildren<CinemachineCamera>();
            if (vcam != null)
            {
                // Store the camera's initial world-space position/rotation
                lastCameraPositions[player] = vcam.transform.position;
                lastCameraRotations[player] = vcam.transform.rotation;
            }
            else
            {
                Debug.LogWarning($"No CinemachineVirtualCamera found in {player.name} or its children.");
            }
        }
    }

    void Update()
    {
        foreach (GameObject player in players)
        {
            // First, do a line-of-sight check to the *player*.
            // This is just one way of doing it—if you prefer line of sight to the camera itself, adjust accordingly.
            Vector3 directionToPlayer = player.transform.position - transform.position;

            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit))
            {
                // If it's truly the player we hit first, then it's not blocked
                if (hit.collider.gameObject == player)
                {
                    // Now check the child's camera, if it exists
                    CinemachineCamera vcam = player.GetComponentInChildren<CinemachineCamera>();
                    if (vcam != null)
                    {
                        Vector3 currentCamPos = vcam.transform.position;    // World-space position
                        Quaternion currentCamRot = vcam.transform.rotation; // World-space rotation

                        float distanceMoved = Vector3.Distance(currentCamPos, lastCameraPositions[player]);
                        float angleMoved = Quaternion.Angle(currentCamRot, lastCameraRotations[player]);

                        if (distanceMoved > positionThreshold || angleMoved > rotationThreshold)
                        {
                            Debug.Log($"Player '{player.name}' has been eliminated (camera moved/rotated).");
                        }

                        // Update the stored camera transform values
                        lastCameraPositions[player] = currentCamPos;
                        lastCameraRotations[player] = currentCamRot;
                    }
                }
                else
                {
                    // Ray got blocked by something else first
                    Debug.Log($"Ray blocked by {hit.collider.gameObject.name} when checking {player.name}. Skipping camera checks.");
                }
            }
            else
            {
                // If there's no hit at all, the ray might be going off into empty space
                Debug.Log($"No collider hit when checking {player.name}.");
            }
        }
    }
}
