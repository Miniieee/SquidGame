using UnityEngine;
using UnityEngine.UI;  // If you want to display UI text

public class RedGreenLightController : MonoBehaviour
{
    [Header("Red/Green Timing (seconds)")]
    [SerializeField] private float greenLightDuration = 3f;  // How long green lasts
    [SerializeField] private float redLightDuration = 3f;    // How long red lasts
    [SerializeField] private PlayerScan playerScan;

    public bool IsRedLight { get; private set; }
    

    private void Start()
    {
        // Start the cycle
        StartCoroutine(RedGreenCycle());
    }

    private System.Collections.IEnumerator RedGreenCycle()
    {
        while (true)
        {
            // GREEN phase
            IsRedLight = false;
            Debug.LogWarning("Green Light");
            yield return new WaitForSeconds(greenLightDuration);

            // RED phase
            playerScan.GetLatestCameraPosition();
            IsRedLight = true;
            Debug.LogWarning("Red Light");
            //UpdateStatusText();
            yield return new WaitForSeconds(redLightDuration);
        }
    }


}