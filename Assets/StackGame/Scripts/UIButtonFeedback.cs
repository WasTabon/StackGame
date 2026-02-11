using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UIButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Tweener scaleTween;
    private Image image;
    private Color originalColor;

    private void Awake()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale * 0.85f, 0.1f).SetEase(Ease.OutQuad);
        if (image != null)
            image.DOColor(originalColor * 0.7f, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack, 2f);
        if (image != null)
            image.DOColor(originalColor, 0.15f);
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }
}
