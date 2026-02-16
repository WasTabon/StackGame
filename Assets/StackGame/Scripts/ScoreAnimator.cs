using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreAnimator : MonoBehaviour
{
    public Text scoreText;

    private int displayedScore = 0;
    private int targetScore = 0;
    private Tweener countTween;
    private Tweener punchTween;

    public void SetScore(int newScore)
    {
        targetScore = newScore;

        countTween?.Kill();
        countTween = DOTween.To(() => displayedScore, x =>
        {
            displayedScore = x;
            scoreText.text = displayedScore.ToString();
        }, targetScore, 0.5f).SetEase(Ease.OutCubic);

        punchTween?.Kill();
        punchTween = scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5);
    }

    private void OnDestroy()
    {
        countTween?.Kill();
        punchTween?.Kill();
    }
}
