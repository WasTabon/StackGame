using UnityEngine;

public class EndlessManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Difficulty Scaling")]
    public float startSpawnInterval = 10f;
    public float minSpawnInterval = 3f;
    public float intervalDecreasePerMinute = 1f;

    private float elapsed = 0f;
    private bool isActive = false;

    public bool IsActive => isActive;

    public void StartEndless()
    {
        isActive = true;
        elapsed = 0f;
        gameManager.spawnInterval = startSpawnInterval;
    }

    private void Update()
    {
        if (!isActive) return;

        elapsed += Time.deltaTime;
        float minutes = elapsed / 60f;
        float newInterval = startSpawnInterval - (minutes * intervalDecreasePerMinute);
        newInterval = Mathf.Max(newInterval, minSpawnInterval);
        gameManager.spawnInterval = newInterval;
    }

    public void SaveHighScore(int score)
    {
        int best = PlayerPrefs.GetInt("EndlessHighScore", 0);
        if (score > best)
        {
            PlayerPrefs.SetInt("EndlessHighScore", score);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("EndlessHighScore", 0);
    }
}
