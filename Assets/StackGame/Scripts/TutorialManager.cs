using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public InputController inputController;
    public GameManager gameManager;
    public CanvasGroup overlayGroup;
    public RectTransform messagePanel;
    public Text messageText;
    public GameObject tapToContinue;

    private int step = 0;
    private bool waitingForTap = false;
    private bool tutorialActive = false;

    private static readonly string[] Messages = new string[]
    {
        "Welcome to STACK!\n\nRotate layers to match colors on the visible side.",
        "Use <b>LEFT / RIGHT</b> buttons\nto rotate the selected layer.",
        "Use <b>UP / DOWN</b> buttons\nto select different layers.",
        "When two adjacent layers have\nmatching colors on the front side\npress <b>CONFIRM</b> to remove them!",
        "Chain reactions give bonus points\nand recharge your power-ups.",
        "New layers spawn from above.\nDon't let the tower get too tall!\n\nGood luck!"
    };

    private void Start()
    {
        bool seen = PlayerPrefs.GetInt("TutorialSeen", 0) == 1;
        if (seen)
        {
            if (overlayGroup != null)
            {
                overlayGroup.alpha = 0f;
                overlayGroup.interactable = false;
                overlayGroup.blocksRaycasts = false;
            }
            return;
        }

        StartTutorial();
    }

    public void StartTutorial()
    {
        tutorialActive = true;
        step = 0;

        if (gameManager != null)
            inputController.SetInputLocked(true);

        ShowStep();
    }

    private void Update()
    {
        if (!tutorialActive) return;
        if (!waitingForTap) return;

        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            waitingForTap = false;
            step++;

            if (step >= Messages.Length)
                EndTutorial();
            else
                ShowStep();
        }
    }

    private void ShowStep()
    {
        overlayGroup.alpha = 0f;
        overlayGroup.interactable = true;
        overlayGroup.blocksRaycasts = true;
        overlayGroup.DOFade(1f, 0.25f);

        messageText.text = Messages[step];

        messagePanel.localScale = Vector3.one * 0.8f;
        messagePanel.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        if (tapToContinue != null)
        {
            CanvasGroup tcg = tapToContinue.GetComponent<CanvasGroup>();
            if (tcg != null)
            {
                tcg.alpha = 0f;
                tcg.DOFade(1f, 0.3f).SetDelay(0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        StartCoroutine(EnableTapAfterDelay(0.4f));
    }

    private IEnumerator EnableTapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        waitingForTap = true;
    }

    private void EndTutorial()
    {
        tutorialActive = false;

        overlayGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            overlayGroup.interactable = false;
            overlayGroup.blocksRaycasts = false;
        });

        if (gameManager != null)
            inputController.SetInputLocked(false);

        PlayerPrefs.SetInt("TutorialSeen", 1);
        PlayerPrefs.Save();
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialSeen");
    }
}
