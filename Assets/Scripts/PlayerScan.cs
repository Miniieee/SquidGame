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

    private GameObject player;
    private CinemachineCamera vcam;
    
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
        player = GameObject.FindGameObjectWithTag("Player");
        vcam = player.GetComponentInChildren<CinemachineCamera>();
    }

    private void Update()
    {
        if (!_isRed || !_canScan) return;
        
        Vector3 directionToPlayer = player.transform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == player)
            {
                Vector3 currentCamPos = vcam.transform.position;
                Vector3 currentCamEuler = vcam.transform.eulerAngles;

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
        if (newState is LightState.GameOver or LightState.Won)
        {
            _canScan = false;
        }
    }
}
