using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpawnTimerUI : MonoBehaviour
{
    public Image fillBar;
    public Image bgBar;

    private static readonly Color NormalColor = new Color(0.18f, 0.78f, 0.85f, 0.8f);
    private static readonly Color WarningColor = new Color(0.90f, 0.22f, 0.35f, 0.9f);

    private Tweener blinkTween;

    public void UpdateTimer(float normalized)
    {
        fillBar.fillAmount = normalized;

        if (normalized > 0.75f)
        {
            fillBar.color = WarningColor;
            if (blinkTween == null || !blinkTween.IsActive())
            {
                blinkTween = fillBar.DOFade(0.4f, 0.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
        else
        {
            fillBar.color = NormalColor;
            if (blinkTween != null && blinkTween.IsActive())
            {
                blinkTween.Kill();
                blinkTween = null;
                Color c = fillBar.color;
                c.a = NormalColor.a;
                fillBar.color = c;
            }
        }
    }

    public void ResetTimer()
    {
        fillBar.fillAmount = 0f;
        fillBar.color = NormalColor;
        if (blinkTween != null)
        {
            blinkTween.Kill();
            blinkTween = null;
        }
    }

    private void OnDestroy()
    {
        blinkTween?.Kill();
    }
}
