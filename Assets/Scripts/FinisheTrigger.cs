using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // If the player reaches the box, they win
            gameManager.SetPlayerWon();
        }
    }
}