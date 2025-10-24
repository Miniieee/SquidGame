using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _gameOver;

    private bool _playerHasWon;

    // Fired whenever the overall game state changes (GameOver, Won)
    public static event Action<LightState> OnGameStateChanged;

    // Call this to end the game (lose) if time ran out or movement was detected, etc.
    // ReSharper disable Unity.PerformanceAnalysis
    public void SetGameOver()
    {
        _gameOver = true;
        Debug.LogError("Game Over " + _playerHasWon);

        if (!_playerHasWon)
        {
            OnGameStateChanged?.Invoke(LightState.GameOver);
            Debug.Log("Game Over: Player is eliminated or time ran out.");
        }
    }

    public void SetPlayerWon()
    {
        if (_gameOver || _playerHasWon) return; // Already resolved

        _playerHasWon = true;
        OnGameStateChanged?.Invoke(LightState.Won);
        Debug.Log("Player Reached Finish: You Win!");
    }

    // Helper if you want a check
    public bool HasPlayerWon()
    {
        return _playerHasWon;
    }

    public bool IsGameOver()
    {
        return _gameOver;
    }
}