using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelectUI : MonoBehaviour
{
    public CanvasGroup panelGroup;
    public RectTransform panelRect;
    public Transform buttonContainer;
    public GameObject backButton;
    public int totalLevels = 10;

    private static readonly Color LockedColor = new Color(0.15f, 0.16f, 0.25f, 1f);
    private static readonly Color UnlockedColor = new Color(0.15f, 0.55f, 0.75f, 1f);
    private static readonly Color CompletedColor = new Color(0.25f, 0.82f, 0.50f, 1f);

    private void Awake()
    {
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        RefreshButtons();

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        panelRect.localScale = Vector3.one * 0.9f;
        panelGroup.DOFade(1f, 0.25f);
        panelRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        panelGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        });
    }

    private void RefreshButtons()
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);

        for (int i = 0; i < buttonContainer.childCount; i++)
        {
            Transform child = buttonContainer.GetChild(i);
            Button btn = child.GetComponent<Button>();
            Image img = child.GetComponent<Image>();
            Text txt = child.GetComponentInChildren<Text>();

            if (i < totalLevels)
            {
                child.gameObject.SetActive(true);
                int levelIdx = i;

                if (i <= unlocked)
                {
                    btn.interactable = true;
                    img.color = i < unlocked ? CompletedColor : UnlockedColor;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnLevelSelected(levelIdx));
                }
                else
                {
                    btn.interactable = false;
                    img.color = LockedColor;
                }

                if (txt != null)
                    txt.text = (i + 1).ToString();
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        PlayerPrefs.Save();
        SceneLoader.Instance.LoadGameplay(SceneLoader.GameMode.Levels);
    }

    public void OnBackPressed()
    {
        Hide();
    }
}
