using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

public class RedGreenLightController : MonoBehaviour
{
    [Header("Durations (seconds)")]

    // We will pick a random red duration in [minGreenTime, maxGreenTime]
    [SerializeField] private float minGreenTime = 2f;
    [SerializeField] private float maxGreenTime = 5f;
    
    // We'll store the actual redLightDuration chosen each cycle
    private float redLightDuration;

    [Header("References")]
    [SerializeField] private PlayerScan playerScan; // if you need to call something on Red
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private HeadRotator headRotator;
    
    [Header("Audio Clips")]
    [FormerlySerializedAs("kokoaudio")]
    [SerializeField] private AudioClip kokoAudio;   // loops during Green
    [SerializeField] private AudioClip turnAudio;   // plays once at start of Red
    
    public bool IsRedLight { get; private set; }

    private void Start()
    {
        // If not assigned in Inspector, try to get it
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(RedGreenCycle());
    }

    private IEnumerator RedGreenCycle()
    {
        while (true)
        {
            // GREEN phase
            IsRedLight = false;
            Debug.LogWarning("Green Light");
            
            headRotator.GreenlightRotateHead();
            
            // Set up kokoAudio to loop for entire Green phase
            
            audioSource.loop = false;
            audioSource.PlayOneShot(kokoAudio);

            // Wait for the green duration
            yield return new WaitForSeconds(kokoAudio.length);

            // End of Green phase: stop looping kokoAudio
            audioSource.Stop();
             // avoid re-looping if we do a one-shot next
            
            // RED phase
            // Pick a random redLightDuration
            redLightDuration = Random.Range(minGreenTime, maxGreenTime);
            IsRedLight = true;
            Debug.LogWarning("Red Light");

            headRotator.RedlightRotateHead();
            // Play turnAudio ONCE at the start of Red
            audioSource.PlayOneShot(turnAudio);

            // If needed, do a position check or something
            if (playerScan != null)
            {
                playerScan.GetLatestCameraPosition();
            }

            // Wait for the red duration
            yield return new WaitForSeconds(redLightDuration);

            // (After waiting, the loop goes back to GREEN)
        }
    }
}
