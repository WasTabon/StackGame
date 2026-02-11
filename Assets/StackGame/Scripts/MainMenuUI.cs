using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public RectTransform titleRect;
    public RectTransform levelsButtonRect;
    public RectTransform endlessButtonRect;
    public CanvasGroup levelsCanvasGroup;
    public CanvasGroup endlessCanvasGroup;

    private void Start()
    {
        AnimateEntrance();
    }

    private void AnimateEntrance()
    {
        titleRect.localScale = Vector3.zero;
        titleRect.DOScale(1f, 0.6f).SetEase(Ease.OutBack, 1.5f).SetDelay(0.2f);

        levelsCanvasGroup.alpha = 0f;
        Vector2 levelsStart = levelsButtonRect.anchoredPosition;
        levelsButtonRect.anchoredPosition = levelsStart + Vector2.down * 60f;
        levelsCanvasGroup.DOFade(1f, 0.4f).SetDelay(0.5f);
        levelsButtonRect.DOAnchorPos(levelsStart, 0.5f).SetEase(Ease.OutCubic).SetDelay(0.5f);

        endlessCanvasGroup.alpha = 0f;
        Vector2 endlessStart = endlessButtonRect.anchoredPosition;
        endlessButtonRect.anchoredPosition = endlessStart + Vector2.down * 60f;
        endlessCanvasGroup.DOFade(1f, 0.4f).SetDelay(0.65f);
        endlessButtonRect.DOAnchorPos(endlessStart, 0.5f).SetEase(Ease.OutCubic).SetDelay(0.65f);
    }

    public void OnLevelsPressed()
    {
        Debug.Assert(SceneLoader.Instance != null, "SceneLoader not found!");
        SceneLoader.Instance.LoadGameplay(SceneLoader.GameMode.Levels);
    }

    public void OnEndlessPressed()
    {
        Debug.Assert(SceneLoader.Instance != null, "SceneLoader not found!");
        SceneLoader.Instance.LoadGameplay(SceneLoader.GameMode.Endless);
    }
}
