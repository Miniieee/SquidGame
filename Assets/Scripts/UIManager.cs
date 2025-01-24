using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    
    private GameManager gameManager;
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        gameOverPanel.SetActive(false);
    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state == LightState.GameOver)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
}
