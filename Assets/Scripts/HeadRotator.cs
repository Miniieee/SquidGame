using UnityEngine;
using DG.Tweening;

public class HeadRotator : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float rotationDuration = 0.6f;

    private Vector3 greenRotation = new Vector3(-90f, 0f, 0f);
    private Vector3 redRotation   = new Vector3(-90f, 0f, -180f);

    private void Start()
    {
        if (head == null) head = transform; // fallback
        head.rotation = Quaternion.Euler(greenRotation);
    }

    public void GreenlightRotateHead()
    {
        head
            .DORotate(greenRotation, rotationDuration)
            .SetEase(Ease.Linear);
    }

    public void RedlightRotateHead()
    {
        head
            .DORotate(redRotation, rotationDuration)
            .SetEase(Ease.Linear);
    }
}