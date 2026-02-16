using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Tower tower;
    public StackChecker stackChecker;
    public InputController inputController;
    public GameOverUI gameOverUI;
    public LevelCompleteUI levelCompleteUI;
    public ScorePopup scorePopup;
    public ParticleSpawner particleSpawner;
    public SpawnTimerUI spawnTimerUI;
    public LevelManager levelManager;
    public EndlessManager endlessManager;

    [Header("Spawn Settings")]
    public float spawnInterval = 8f;
    public int maxLayers = 10;

    [Header("Score")]
    public Text scoreText;

    private float spawnTimer;
    private int score = 0;
    private bool gameOver = false;

    private void Start()
    {
        stackChecker.OnLayersRemoved += OnLayersRemoved;
        stackChecker.OnLayerRemoving += OnLayerRemoving;

        if (SceneLoader.Instance != null && SceneLoader.Instance.CurrentMode == SceneLoader.GameMode.Levels)
        {
            if (levelManager != null)
            {
                int levelIdx = PlayerPrefs.GetInt("SelectedLevel", 0);
                levelManager.StartLevel(levelIdx);
                tower.SpawnInitialLayers();
            }
        }
        else
        {
            if (endlessManager != null)
                endlessManager.StartEndless();
            tower.SpawnInitialLayers();
        }

        spawnTimer = spawnInterval;
        UpdateScoreUI();
        if (spawnTimerUI != null)
            spawnTimerUI.ResetTimer();

        inputController.RefreshSelection();
    }

    private void OnDestroy()
    {
        if (stackChecker != null)
        {
            stackChecker.OnLayersRemoved -= OnLayersRemoved;
            stackChecker.OnLayerRemoving -= OnLayerRemoving;
        }
    }

    private void Update()
    {
        if (gameOver) return;
        if (stackChecker.IsProcessing) return;
        if (levelManager != null && levelManager.IsLevelComplete) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimerUI != null)
        {
            float normalized = 1f - (spawnTimer / spawnInterval);
            spawnTimerUI.UpdateTimer(normalized);
        }

        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnNewLayer();
            if (spawnTimerUI != null)
                spawnTimerUI.ResetTimer();
        }
    }

    private void SpawnNewLayer()
    {
        BlockLayer newLayer = tower.AddLayerOnTop();

        float targetY = newLayer.transform.localPosition.y;
        newLayer.transform.localPosition = new Vector3(0f, targetY + 2f, 0f);
        newLayer.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Join(newLayer.transform.DOLocalMoveY(targetY, 0.4f).SetEase(Ease.OutBounce));
        seq.Join(newLayer.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

        inputController.RefreshSelection();
        CheckGameOver();
    }

    private void OnLayerRemoving(BlockLayer layer)
    {
        if (particleSpawner != null)
            particleSpawner.SpawnForLayer(layer);
    }

    private void OnLayersRemoved(int removedCount, int chainStep, Vector3 avgPosition)
    {
        int chainBonus = chainStep > 1 ? chainStep * 2 : 1;
        int points = removedCount * 100 * chainBonus;
        score += points;
        UpdateScoreUI();

        if (scorePopup != null)
            scorePopup.ShowAt(avgPosition, points, chainStep);

        spawnTimer = spawnInterval;
        if (spawnTimerUI != null)
            spawnTimerUI.ResetTimer();

        if (levelManager != null && levelManager.IsActive)
        {
            levelManager.OnLayersRemoved(removedCount, chainStep);
            levelManager.OnScoreChanged(score);
        }
    }

    private void CheckGameOver()
    {
        if (tower.layers.Count >= maxLayers)
        {
            gameOver = true;
            inputController.SetInputLocked(true);

            if (endlessManager != null && endlessManager.IsActive)
                endlessManager.SaveHighScore(score);

            gameOverUI.Show(score);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void ShowLevelComplete(bool hasNext)
    {
        inputController.SetInputLocked(true);
        if (levelCompleteUI != null)
            levelCompleteUI.Show(score, hasNext);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void Retry()
    {
        SceneLoader.Instance.LoadGameplay(SceneLoader.Instance.CurrentMode);
    }

    public void GoToMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }

    public void NextLevel()
    {
        if (levelManager != null)
        {
            int next = levelManager.GetCurrentLevelIndex() + 1;
            PlayerPrefs.SetInt("SelectedLevel", next);
            PlayerPrefs.Save();
        }
        SceneLoader.Instance.LoadGameplay(SceneLoader.GameMode.Levels);
    }
}
