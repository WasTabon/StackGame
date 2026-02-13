using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Tower tower;
    public StackChecker stackChecker;
    public InputController inputController;
    public GameOverUI gameOverUI;
    public ScorePopup scorePopup;
    public ParticleSpawner particleSpawner;
    public SpawnTimerUI spawnTimerUI;

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
        spawnTimer = spawnInterval;
        UpdateScoreUI();
        stackChecker.OnLayersRemoved += OnLayersRemoved;
        stackChecker.OnLayerRemoving += OnLayerRemoving;
        if (spawnTimerUI != null)
            spawnTimerUI.ResetTimer();
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
    }

    private void CheckGameOver()
    {
        if (tower.layers.Count >= maxLayers)
        {
            gameOver = true;
            inputController.SetInputLocked(true);
            gameOverUI.Show(score);
        }
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
}
