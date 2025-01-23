using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 300f; // 5 minutes
    private TextMeshProUGUI timerText;

    [Header("References")]
    [SerializeField] private GameManager gameManager; // Assign in inspector

    private bool _timeIsUp;

    void Start()
    {
        // Find the TextMeshProUGUI on this object or a child
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (_timeIsUp) return;
        if (timeRemaining <= 0) return;

        // Decrease time
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            _timeIsUp = true;

            // If the player hasn't won yet, declare game over
            if (!gameManager.HasPlayerWon())
            {
                gameManager.SetGameOver();
            }
        }

        // Update UI
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        if (timerText != null)
        {
            timerText.text = $"{minutes}:{seconds:00}";
        }
    }
}