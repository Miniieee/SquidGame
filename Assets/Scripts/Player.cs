using System;
using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
     //private GameManager gameManager;
    private CinemachineInputAxisController inputAxisController;
    private PlayerMovement playerMovement;
    private bool hasReachedFinish;
    public bool HasReachedFinish {get => hasReachedFinish; set => hasReachedFinish = value;}
    
    void Start()
    {
        //gameManager = FindFirstObjectByType<GameManager>();
        
    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state is LightState.GameOver or LightState.Won)
        {
            inputAxisController = GetComponentInChildren<CinemachineInputAxisController>();
            playerMovement = GetComponent<PlayerMovement>();
            inputAxisController.enabled = false;
            playerMovement.enabled = false;
        }

        //disable xr movement
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
