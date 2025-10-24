using UnityEngine;
using DG.Tweening;

public class ObsticalMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float leftPosition = -25f;
    [SerializeField] private float rightPosition = 25f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Ease ease = Ease.Linear;
    
    private Tweener myTweener;

    private void Start()
    {
        // Optionally set the object to start at the left position:
        transform.position = new Vector3(leftPosition, transform.position.y, transform.position.z);

        // Move from left to right in 'duration' seconds, then back, indefinitely
        myTweener = transform.DOMoveX(rightPosition, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo); 
    }
    
    private void OnDestroy()
    {
        // Kill the tween referencing this object's Transform.
        if (myTweener != null && myTweener.IsActive())
        {
            myTweener.Kill();
        }
    }
}
