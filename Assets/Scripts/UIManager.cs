using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [FormerlySerializedAs("gameOverPanel")] [SerializeField] private GameObject lostPanel;
    [SerializeField] private GameObject winPanel;
    
    private GameManager gameManager;
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        lostPanel.SetActive(false);
    }

    private void HandleGameStateChanged(LightState state)
    {
        if (state == LightState.GameOver)
        {
            lostPanel.SetActive(true);
        }

        if (state == LightState.Won)
        {
            winPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
}
