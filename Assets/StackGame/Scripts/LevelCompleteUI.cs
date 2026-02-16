using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelCompleteUI : MonoBehaviour
{
    public CanvasGroup panelGroup;
    public RectTransform panelRect;
    public Text titleText;
    public Text scoreText;
    public GameObject nextButton;
    public GameObject menuButton;
    public GameObject retryButton;

    private void Awake()
    {
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
    }

    public void Show(int score, bool hasNextLevel)
    {
        titleText.text = "LEVEL COMPLETE!";
        scoreText.text = "SCORE\n" + score;

        nextButton.SetActive(hasNextLevel);

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
