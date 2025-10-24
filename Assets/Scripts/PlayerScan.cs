using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PlayerScan : MonoBehaviour
{
    [Header("Thresholds")]
    [SerializeField] private float positionThreshold = 0.10f; // meters
    [SerializeField] private float rotationThreshold = 5f;     // degrees

    [Header("Layer Mask")]
    [SerializeField] private LayerMask layerMask = ~0;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    private XROrigin _xrOrigin;            // Player root (XR rig)
    private Transform _headObject;        // Usually xrOrigin.Camera.transform
    [SerializeField] private Transform _leftHandObject;    // Controller/hand transform
    [SerializeField] private Transform _rightHandObject;   // Controller/hand transform
    private bool trackHands = true;


    // Optional: still use your Player script (for HasReachedFinish)
    private Player player;

    // Snapshot pose when Red turns on
    private Vector3 _lastHeadPos;
    private Quaternion _lastHeadRot;

    private Vector3 _lastLeftPos;
    private Quaternion _lastLeftRot;

    private Vector3 _lastRightPos;
    private Quaternion _lastRightRot;

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

    private async void Start()
    {
        _xrOrigin = FindFirstObjectByType<XROrigin>();
        _headObject = GameObject.FindGameObjectWithTag("MainCamera").transform;

        player = _xrOrigin.GetComponent<Player>();
    }

   void Update()
{
    if (!_isRed || !_canScan) return;
    if (_headObject == null || _xrOrigin == null) return;
    if (player != null && player.HasReachedFinish) return;

    Vector3 toHead = _headObject.position - transform.position;
    float distToHead = toHead.magnitude;
    Vector3 dir = toHead / distToHead;

    // Raycast up to the head distance; if we hit something BEFORE the head that's not part of the rig, it's occluded.
    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distToHead, layerMask, QueryTriggerInteraction.Ignore))
    {
        if (!hit.transform.IsChildOf(_xrOrigin.transform))
        {
            // Something else is in front of the head -> occluded; skip this frame.
            return;
        }
    }
    // If we didn’t hit anything up to the head, assume clear line of sight.

    bool moved =
        PositionDeltaExceeded(_headObject.position, _lastHeadPos) ||
        RotationDeltaExceeded(_headObject.rotation, _lastHeadRot);

    if (trackHands)
    {
        if (_leftHandObject != null)
            moved |= PositionDeltaExceeded(_leftHandObject.position, _lastLeftPos) ||
                     RotationDeltaExceeded(_leftHandObject.rotation, _lastLeftRot);

        if (_rightHandObject != null)
            moved |= PositionDeltaExceeded(_rightHandObject.position, _lastRightPos) ||
                     RotationDeltaExceeded(_rightHandObject.rotation, _lastRightRot);
    }

    if (moved)
    {
        Debug.Log("Player eliminated (moved during Red).");
        gameManager?.SetGameOver();
        _canScan = false;
    }
}

     private void OnLightStateChanged(LightState newState)
    {
        if (newState == LightState.Red)
        {
            _isRed = true;

            if (_headObject == null) return;

            // Snapshot current poses
            _lastHeadPos = _headObject.position;
            _lastHeadRot = _headObject.rotation;

            if (trackHands && _leftHandObject != null)
            {
                _lastLeftPos = _leftHandObject.position;
                _lastLeftRot = _leftHandObject.rotation;
            }
            if (trackHands && _rightHandObject != null)
            {
                _lastRightPos = _rightHandObject.position;
                _lastRightRot = _rightHandObject.rotation;
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

    // ---- Helpers ----

    private bool PositionDeltaExceeded(Vector3 current, Vector3 last)
    {
        // Spherical distance feels better in VR than per-axis thresholds
        return Vector3.Distance(current, last) > positionThreshold;
    }

    private bool RotationDeltaExceeded(Quaternion current, Quaternion last)
    {
        return Quaternion.Angle(last, current) > rotationThreshold;
    }
}
