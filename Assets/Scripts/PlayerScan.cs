using UnityEngine;
using Unity.Cinemachine;  // If using Cinemachine; otherwise adjust accordingly

public class PlayerScan : MonoBehaviour
{
    [Header("Thresholds")]
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 5f; // degrees per Euler axis
    
    [Header("Layer Setup")]
    [SerializeField] private string groundLayerName = "Ground";
    private int groundLayerIndex;
    private int layerMaskExcludingGround;
    
    [SerializeField] RedGreenLightController redGreenLightController;

    private GameObject player;
    private CinemachineCamera vcam;  // Or CinemachineCamera if you have a custom script
    
    // Store the last-known camera position/rotation
    private Vector3 lastCameraPos;
    private Vector3 lastCameraEuler;

    private void Awake()
    {
        // Find the layer index by name
        groundLayerIndex = LayerMask.NameToLayer(groundLayerName);
        if (groundLayerIndex == -1)
        {
            Debug.LogError($"No layer named '{groundLayerName}' found. Check your Project Settings → Tags and Layers.");
        }

        // Create a mask that excludes the Ground layer
        layerMaskExcludingGround = groundLayerIndex;
    }

    private void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene!");
            enabled = false;
            return;
        }

        // Find the Cinemachine Virtual Camera in the player's children
        vcam = player.GetComponentInChildren<CinemachineCamera>();
        if (vcam == null)
        {
            Debug.LogError($"No CinemachineVirtualCamera found as a child of '{player.name}'.");
            enabled = false;
            return;
        }

        // Initialize last-known camera position and rotation (in Euler angles)
        lastCameraPos = vcam.transform.position;
        lastCameraEuler = vcam.transform.eulerAngles;
    }
    
    public void GetLatestCameraPosition()
    {
        lastCameraPos = vcam.transform.position;
        lastCameraEuler = vcam.transform.eulerAngles;
    }

    private void Update()
    {
        
        if(!redGreenLightController.IsRedLight) return;
        // Direction from this object to the player's main transform
        Vector3 directionToPlayer = player.transform.position - transform.position;

        // Raycast, ignoring Ground layer
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~layerMaskExcludingGround))
        {
            // If the first thing we hit is the player, it's not blocked
            if (hit.collider.gameObject == player)
            {
                // Check the camera's current position/rotation
                Vector3 currentCamPos = vcam.transform.position;
                Vector3 currentCamEuler = vcam.transform.eulerAngles;

                // Compare each position axis
                bool movedOnX = Mathf.Abs(currentCamPos.x - lastCameraPos.x) > positionThreshold;
                bool movedOnY = Mathf.Abs(currentCamPos.y - lastCameraPos.y) > positionThreshold;
                bool movedOnZ = Mathf.Abs(currentCamPos.z - lastCameraPos.z) > positionThreshold;

                bool positionChanged = (movedOnX || movedOnY || movedOnZ);

                // Compare each rotation axis (Euler angles)
                bool rotatedOnX = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.x, currentCamEuler.x)) > rotationThreshold;
                bool rotatedOnY = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.y, currentCamEuler.y)) > rotationThreshold;
                bool rotatedOnZ = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.z, currentCamEuler.z)) > rotationThreshold;

                bool rotationChanged = (rotatedOnX || rotatedOnY || rotatedOnZ);

                if (positionChanged || rotationChanged)
                {
                    Debug.Log($"Player '{player.name}' eliminated (camera moved/rotated).");
                    // Place any elimination logic here (destroy player, disable components, etc.)
                }
                
            }
            else
            {
                // The ray is blocked by something else
                Debug.Log($"Ray blocked by '{hit.collider.gameObject.name}'. Skipping checks on player '{player.name}'.");
            }
        }
        else
        {
            // We didn't hit anything (excluding the Ground layer)
            Debug.Log($"No hit (excluding '{groundLayerName}' layer) for player '{player.name}'.");
        }
    }
}
