using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class RedGreenLightController : MonoBehaviour
{
    public static event Action<LightState> OnLightStateChanged;
    
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
    [SerializeField] private AudioClip greenLoopAudio;
    [SerializeField] private AudioClip redStartAudio;

    [Header("Head Rotation")]
    [SerializeField] private HeadRotator headRotator;

    private bool _stopCycle;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        StartCoroutine(RedGreenCycle());
    }

    private IEnumerator RedGreenCycle()
    {
        while (!_stopCycle)
        {
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
                yield return new WaitForSeconds(3f);
            }

            if (_stopCycle) yield break;
            
            CurrentState = LightState.Red;
            Debug.Log("Red Light");
            if (headRotator != null) headRotator.RedlightRotateHead();

            if (redStartAudio != null && audioSource != null)
            {
                audioSource.PlayOneShot(redStartAudio);
            }
            
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
