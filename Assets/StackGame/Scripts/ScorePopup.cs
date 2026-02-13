using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScorePopup : MonoBehaviour
{
    public Canvas canvas;
    public Camera gameCamera;

    private static readonly Color NormalColor = new Color(0.85f, 0.90f, 1f, 1f);
    private static readonly Color ChainColor = new Color(0.95f, 0.55f, 0.20f, 1f);
    private static readonly Color BigChainColor = new Color(0.90f, 0.22f, 0.35f, 1f);

    public void ShowAt(Vector3 worldPos, int points, int chainStep)
    {
        GameObject obj = new GameObject("ScorePopup");
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300f, 80f);

        Vector2 screenPos = gameCamera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(), screenPos, null, out Vector2 localPos);
        rect.anchoredPosition = localPos;

        Text text = obj.AddComponent<Text>();
        text.text = "+" + points;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        if (chainStep >= 3)
        {
            text.fontSize = 52;
            text.color = BigChainColor;
            text.text = "+" + points + " x" + chainStep;
        }
        else if (chainStep >= 2)
        {
            text.fontSize = 44;
            text.color = ChainColor;
            text.text = "+" + points + " x" + chainStep;
        }
        else
        {
            text.fontSize = 36;
            text.color = NormalColor;
        }

        CanvasGroup cg = obj.AddComponent<CanvasGroup>();

        rect.localScale = Vector3.one * 0.5f;
        rect.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        rect.DOAnchorPosY(rect.anchoredPosition.y + 120f, 0.8f).SetEase(Ease.OutQuad);
        cg.DOFade(0f, 0.4f).SetDelay(0.5f).OnComplete(() =>
        {
            Destroy(obj);
        });
    }
}
