using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class RedGreenLightController : MonoBehaviour
{
    // This event notifies about any *light state* changes (Green, Red, etc.)
    public static event Action<LightState> OnLightStateChanged;

    // Current LightState
    private LightState _currentState;
    public LightState CurrentState
    {
        get => _currentState;
        private set
        {
            _currentState = value;
            OnLightStateChanged?.Invoke(_currentState);
        }
    }

    [Header("Durations (seconds)")]
    [SerializeField] private float minRedDuration = 2f;
    [SerializeField] private float maxRedDuration = 5f;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip greenLoopAudio; // Plays for entire Green phase
    [SerializeField] private AudioClip redStartAudio;  // Plays once at start of Red

    [Header("Head Rotation")]
    [SerializeField] private HeadRotator headRotator;

    private bool _stopCycle; // If game is over or won, we stop the cycle.

    private void OnEnable()
    {
        // Listen for external signals (like GameOver or Won).
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        // If audioSource not assigned, try local
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Start the repeated cycle
        StartCoroutine(RedGreenCycle());
    }

    private IEnumerator RedGreenCycle()
    {
        // Repeats as long as the game isn't over or won
        while (!_stopCycle)
        {
            // --- GREEN PHASE ---
            CurrentState = LightState.Green;
            Debug.Log("Green Light");
            if (headRotator != null) headRotator.GreenlightRotateHead();

            if (greenLoopAudio != null && audioSource != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(greenLoopAudio);
                yield return new WaitForSeconds(greenLoopAudio.length);
                audioSource.Stop();
            }
            else
            {
                // If no clip assigned, just wait a default 3 seconds
                yield return new WaitForSeconds(3f);
            }

            if (_stopCycle) yield break; // break if game ended mid-phase

            // --- RED PHASE ---
            CurrentState = LightState.Red;
            Debug.Log("Red Light");
            if (headRotator != null) headRotator.RedlightRotateHead();

            if (redStartAudio != null && audioSource != null)
            {
                audioSource.PlayOneShot(redStartAudio);
            }

            // Pick random Red duration
            float redDuration = Random.Range(minRedDuration, maxRedDuration);
            float timer = 0f;
            while (timer < redDuration && !_stopCycle)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void HandleGameStateChanged(LightState newState)
    {

        if (newState is LightState.GameOver or LightState.Won)
        {
            _stopCycle = true;
            // Optionally set CurrentState = newState if you want to finalize.
            // Or leave it as Red if it was in the middle of Red.
        }
    }
}
