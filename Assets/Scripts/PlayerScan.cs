using UnityEngine;
using Unity.Cinemachine;

public class PlayerScan : MonoBehaviour
{
    [Header("Thresholds")]
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 5f; // degrees

    [Header("Layer Mask")]
    [SerializeField] private LayerMask layerMask; // e.g. everything except ground

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private GameObject player;
    private CinemachineCamera vcam;

    // Last known camera pos/rot when Red starts
    private Vector3 lastCameraPos;
    private Vector3 lastCameraEuler;

    private bool _isRed;         // Are we currently in Red state?
    private bool _canScan = true; // If game is over or won, scanning stops

    private void OnEnable()
    {
        // Listen for Red/Green changes
        RedGreenLightController.OnLightStateChanged += OnLightStateChanged;

        // Listen for overall game states (GameOver, Won)
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        RedGreenLightController.OnLightStateChanged -= OnLightStateChanged;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        // Locate player
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found!");
            enabled = false;
            return;
        }

        // Locate Cinemachine camera
        vcam = player.GetComponentInChildren<CinemachineCamera>();
        if (vcam == null)
        {
            Debug.LogError("No CinemachineCamera found as a child of Player.");
            enabled = false;
            return;
        }

        // If no gameManager assigned, try find one
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        // Only scan if: 
        // 1) It's Red light, AND 
        // 2) The game hasn't ended (GameOver or Won).
        if (!_isRed || !_canScan) return;

        // Raycast from this object (the doll or vantage point) toward the player
        Vector3 directionToPlayer = player.transform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == player)
            {
                // Compare current cam position/rotation to stored
                Vector3 currentCamPos = vcam.transform.position;
                Vector3 currentCamEuler = vcam.transform.eulerAngles;

                bool positionChanged =
                    Mathf.Abs(currentCamPos.x - lastCameraPos.x) > positionThreshold ||
                    Mathf.Abs(currentCamPos.y - lastCameraPos.y) > positionThreshold ||
                    Mathf.Abs(currentCamPos.z - lastCameraPos.z) > positionThreshold;

                // Rotation check using DeltaAngle
                bool rotatedOnX = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.x, currentCamEuler.x)) > rotationThreshold;
                bool rotatedOnY = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.y, currentCamEuler.y)) > rotationThreshold;
                bool rotatedOnZ = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.z, currentCamEuler.z)) > rotationThreshold;

                bool rotationChanged = (rotatedOnX || rotatedOnY || rotatedOnZ);

                if (positionChanged || rotationChanged)
                {
                    Debug.Log("Player eliminated (moved during Red).");
                    // Tell GameManager => game over
                    gameManager.SetGameOver();
                }
            }
            else
            {
                Debug.Log($"Ray blocked by {hit.collider.gameObject.name}, skipping move-check.");
            }
        }
    }

    private void OnLightStateChanged(LightState newState)
    {
        // If we turned Red, record camera pos/rot
        // If we turned Green, do nothing. 
        // If we turned GameOver/Won, that is handled by OnGameStateChanged.

        if (newState == LightState.Red)
        {
            _isRed = true;

            // Store camera pos/rot
            if (vcam != null)
            {
                lastCameraPos = vcam.transform.position;
                lastCameraEuler = vcam.transform.eulerAngles;
            }
        }
        else
        {
            _isRed = false;
        }
    }

    private void OnGameStateChanged(LightState newState)
    {
        // If the game is either over or won, we stop scanning
        if (newState == LightState.GameOver || newState == LightState.Won)
        {
            _canScan = false;
        }
    }
}
