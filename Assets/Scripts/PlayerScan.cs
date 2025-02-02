using UnityEngine;
using Unity.Cinemachine;

public class PlayerScan : MonoBehaviour
{
    [Header("Thresholds")]
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 5f;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask layerMask;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private GameObject playerObject;
    private CinemachineCamera virtualCam;
    private Player player;
    
    private Vector3 lastCameraPos;
    private Vector3 lastCameraEuler;

    private bool _isRed;
    private bool _canScan = true;

    private void OnEnable()
    {
        RedGreenLightController.OnLightStateChanged += OnLightStateChanged;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        RedGreenLightController.OnLightStateChanged -= OnLightStateChanged;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        virtualCam = playerObject.GetComponentInChildren<CinemachineCamera>();
        player = playerObject.GetComponent<Player>();
    }

    private void Update()
    {
        if (!_isRed || !_canScan) return;
        
        Vector3 directionToPlayer = playerObject.transform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if(player.HasReachedFinish) return;
            
            if (hit.collider.gameObject == playerObject)
            {
                Vector3 currentCamPos = virtualCam.transform.position;
                Vector3 currentCamEuler = virtualCam.transform.eulerAngles;

                bool positionChanged =
                    Mathf.Abs(currentCamPos.x - lastCameraPos.x) > positionThreshold ||
                    Mathf.Abs(currentCamPos.y - lastCameraPos.y) > positionThreshold ||
                    Mathf.Abs(currentCamPos.z - lastCameraPos.z) > positionThreshold;

                bool rotatedOnX = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.x, currentCamEuler.x)) > rotationThreshold;
                bool rotatedOnY = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.y, currentCamEuler.y)) > rotationThreshold;
                bool rotatedOnZ = Mathf.Abs(Mathf.DeltaAngle(lastCameraEuler.z, currentCamEuler.z)) > rotationThreshold;

                bool rotationChanged = (rotatedOnX || rotatedOnY || rotatedOnZ);

                if (positionChanged || rotationChanged)
                {
                    Debug.Log("Player eliminated (moved during Red).");
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
        if (newState == LightState.Red)
        {
            _isRed = true;

            if (virtualCam == null) return;
            
            lastCameraPos = virtualCam.transform.position;
            lastCameraEuler = virtualCam.transform.eulerAngles;
        }
        else
        {
            _isRed = false;
        }
    }

    private void OnGameStateChanged(LightState newState)
    {
        if (newState is LightState.GameOver or LightState.Won)
        {
            _canScan = false;
        }
    }
}
