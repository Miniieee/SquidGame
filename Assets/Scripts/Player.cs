using System;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class Player : MonoBehaviour
{
     //private GameManager gameManager;
    private bool hasReachedFinish;
    public bool HasReachedFinish {get => hasReachedFinish; set => hasReachedFinish = value;}
    private ContinuousMoveProvider _continuousMoveProvider;

    void Start()
    {
        _continuousMoveProvider = GetComponentInChildren<ContinuousMoveProvider>();
        _continuousMoveProvider.enabled = true;

    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state is LightState.GameOver)
        {
            _continuousMoveProvider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine"))
        {
            hasReachedFinish = true;
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

}
