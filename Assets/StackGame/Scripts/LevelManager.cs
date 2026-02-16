using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public GameManager gameManager;
    public Tower tower;
    public Text goalText;
    public Text progressText;
    public RectTransform goalPanel;
    public LevelData[] levels;

    private LevelData currentLevel;
    private int currentLevelIndex = 0;
    private int removedLayers = 0;
    private int maxChain = 0;
    private float surviveTimer = 0f;
    private bool levelComplete = false;
    private bool isActive = false;

    public bool IsActive => isActive;
    public bool IsLevelComplete => levelComplete;

    public void StartLevel(int levelIndex)
    {
        if (levels == null || levelIndex >= levels.Length)
        {
            Debug.LogError("[LevelManager] No level data for index " + levelIndex);
            return;
        }

        currentLevelIndex = levelIndex;
        currentLevel = levels[levelIndex];
        levelComplete = false;
        isActive = true;
        removedLayers = 0;
        maxChain = 0;
        surviveTimer = 0f;

        gameManager.spawnInterval = currentLevel.spawnInterval;
        gameManager.maxLayers = currentLevel.maxLayers;
        tower.initialLayerCount = currentLevel.startingLayers;
        GameColors.SetActiveColorCount(currentLevel.colorCount);

        UpdateGoalUI();
        ShowGoalPanel();
    }

    private void Update()
    {
        if (!isActive || levelComplete) return;

        if (currentLevel.goalType == LevelData.GoalType.SurviveTime)
        {
            surviveTimer += Time.deltaTime;
            UpdateProgressUI();
            if (surviveTimer >= currentLevel.goalValue)
                CompleteLevel();
        }
    }

    public void OnLayersRemoved(int count, int chainStep)
    {
        if (!isActive || levelComplete) return;

        removedLayers += count;
        if (chainStep > maxChain)
            maxChain = chainStep;

        UpdateProgressUI();
        CheckGoal();
    }

    public void OnScoreChanged(int score)
    {
        if (!isActive || levelComplete) return;
        if (currentLevel.goalType == LevelData.GoalType.ReachScore)
        {
            UpdateProgressUI();
            CheckGoal();
        }
    }

    private void CheckGoal()
    {
        switch (currentLevel.goalType)
        {
            case LevelData.GoalType.RemoveLayers:
                if (removedLayers >= currentLevel.goalValue)
                    CompleteLevel();
                break;
            case LevelData.GoalType.ReachScore:
                if (gameManager.GetScore() >= currentLevel.goalValue)
                    CompleteLevel();
                break;
            case LevelData.GoalType.ChainReaction:
                if (maxChain >= currentLevel.goalValue)
                    CompleteLevel();
                break;
        }
    }

    private void CompleteLevel()
    {
        levelComplete = true;
        int nextIndex = currentLevelIndex + 1;
        bool hasNext = nextIndex < levels.Length;

        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (nextIndex > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextIndex);
            PlayerPrefs.Save();
        }

        gameManager.ShowLevelComplete(hasNext);
    }

    private void UpdateGoalUI()
    {
        if (goalText != null)
            goalText.text = "LEVEL " + (currentLevelIndex + 1) + "\n" + currentLevel.GetGoalDescription();
    }

    private void UpdateProgressUI()
    {
        if (progressText == null) return;

        switch (currentLevel.goalType)
        {
            case LevelData.GoalType.RemoveLayers:
                progressText.text = removedLayers + " / " + currentLevel.goalValue;
                break;
            case LevelData.GoalType.ReachScore:
                progressText.text = gameManager.GetScore() + " / " + currentLevel.goalValue;
                break;
            case LevelData.GoalType.SurviveTime:
                int remaining = Mathf.Max(0, currentLevel.goalValue - (int)surviveTimer);
                progressText.text = remaining + "s";
                break;
            case LevelData.GoalType.ChainReaction:
                progressText.text = "Best: x" + maxChain + " / x" + currentLevel.goalValue;
                break;
        }
    }

    private void ShowGoalPanel()
    {
        if (goalPanel == null) return;

        goalPanel.localScale = Vector3.zero;
        goalPanel.gameObject.SetActive(true);
        goalPanel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                goalPanel.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    goalPanel.gameObject.SetActive(false);
                });
            });
        });
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}
