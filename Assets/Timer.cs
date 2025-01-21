using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Set this to 300 for 5:00 (5 minutes * 60 seconds)
    [SerializeField] private float timeRemaining = 300f;
    
    private TextMeshProUGUI timerText;

    void Start()
    {
        // Get the TextMeshProUGUI component on the same GameObject
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        // Only decrease time if it's above 0
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            // Clamp the time to 0, so it doesn't go negative
            if (timeRemaining < 0)
            {
                timeRemaining = 0;
            }
        }

        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Update the text in the format "M:SS"
        timerText.text = $"{minutes}:{seconds:00}";
    }
}
