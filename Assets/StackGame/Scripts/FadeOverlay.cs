using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FadeOverlay : MonoBehaviour
{
    private Image fadeImage;
    private Canvas fadeCanvas;
    private Tweener currentTween;

    private void Awake()
    {
        fadeCanvas = GetComponentInParent<Canvas>();
        fadeImage = GetComponent<Image>();
        Debug.Assert(fadeImage != null, "FadeOverlay requires an Image component!");
    }

    public void FadeToBlack(float duration, Action onComplete = null)
    {
        fadeImage.raycastTarget = true;
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        gameObject.SetActive(true);

        currentTween?.Kill();
        currentTween = fadeImage.DOFade(1f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public void FadeFromBlack(float duration, Action onComplete = null)
    {
        fadeImage.color = Color.black;
        gameObject.SetActive(true);

        currentTween?.Kill();
        currentTween = fadeImage.DOFade(0f, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                fadeImage.raycastTarget = false;
                onComplete?.Invoke();
            });
    }

    public void SetBlack()
    {
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = true;
        gameObject.SetActive(true);
    }

    public void SetClear()
    {
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
