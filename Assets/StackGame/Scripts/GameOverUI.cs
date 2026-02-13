using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup panelGroup;
    public Text finalScoreText;
    public RectTransform panelRect;

    private void Awake()
    {
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
    }

    public void Show(int score)
    {
        finalScoreText.text = "SCORE\n" + score;

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;

        panelRect.localScale = Vector3.one * 0.8f;
        panelGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
        panelRect.DOScale(1f, 0.35f).SetEase(Ease.OutBack, 1.3f);
    }

    public void Hide()
    {
        panelGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        });
    }
}
