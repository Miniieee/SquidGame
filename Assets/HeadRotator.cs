using UnityEngine;
using DG.Tweening; // Don't forget this namespace for DOTween

public class HeadRotator : MonoBehaviour
{
    [SerializeField] private GameObject head;

    // Duration of the rotation in seconds
    private float rotationDuration = 0.6f;

    private void Start()
    {

        head.transform.rotation = Quaternion.Euler(new Vector3(-90f,0f,0f));
    }

    public void RedlightRotateHead()
    {

        head.transform
            .DORotate(new Vector3(-90f, 0, -180f), rotationDuration)
            .SetEase(Ease.Linear);
    }
    
    public void GreenlightRotateHead()
    {

        head.transform
            .DORotate(new Vector3(-90f,0f,0f), rotationDuration)
            .SetEase(Ease.Linear);
    }
}
