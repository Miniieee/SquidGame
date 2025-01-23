using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // Fired whenever the overall game state changes (GameOver, Won)
    public static event Action<LightState> OnGameStateChanged;

    private bool _playerHasWon;
    private bool _gameOver;

    // Call this to end the game (lose) if time ran out or movement was detected, etc.
    public void SetGameOver()
    {
        if (_playerHasWon || _gameOver) return; // Already resolved

        _gameOver = true;
        OnGameStateChanged?.Invoke(LightState.GameOver);
        Debug.Log("Game Over: Player is eliminated or time ran out.");
    }

    // Call this when the player crosses the finish line
    public void SetPlayerWon()
    {
        if (_gameOver || _playerHasWon) return; // Already resolved

        _playerHasWon = true;
        OnGameStateChanged?.Invoke(LightState.Won);
        Debug.Log("Player Reached Finish: You Win!");
    }

    // Helper if you want a check
    public bool HasPlayerWon() => _playerHasWon;
    public bool IsGameOver() => _gameOver;
}