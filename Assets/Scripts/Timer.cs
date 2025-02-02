using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 300f;
    private TextMeshProUGUI timerText;

    [Header("References")]
    [SerializeField] private GameManager gameManager; 

    private bool _timeIsUp;

    void Start()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (_timeIsUp) return;
        if (timeRemaining <= 0) return;
        
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            _timeIsUp = true;
            
            gameManager.SetGameOver();
        }
        
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        if (timerText != null)
        {
            timerText.text = $"{minutes}:{seconds:00}";
        }
    }
}