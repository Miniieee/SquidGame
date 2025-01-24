using System;
using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
     //private GameManager gameManager;
    private CinemachineInputAxisController inputAxisController;
    
    void Start()
    {
        //gameManager = FindFirstObjectByType<GameManager>();
        
    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state == LightState.GameOver)
        {
            inputAxisController = GetComponentInChildren<CinemachineInputAxisController>();
            inputAxisController.enabled = false;
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
