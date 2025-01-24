using System;
using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private CinemachineInputAxisController inputAxisController;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        inputAxisController = GetComponentInChildren<CinemachineInputAxisController>();
        
        inputAxisController.enabled = true;
        
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state == LightState.GameOver)
        {
            inputAxisController.enabled = false;
        }
    }
}
