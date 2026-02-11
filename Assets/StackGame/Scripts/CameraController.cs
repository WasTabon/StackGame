using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Tower tower;
    public float distance = 4.5f;
    public float heightOffset = 0.3f;
    public float pitchAngle = 25f;
    public float yawAngle = 35f;
    public float followDuration = 0.35f;

    private Tweener moveTween;
    private Tweener lookTween;
    private Vector3 currentLookTarget;

    public void FocusOnTowerCenter()
    {
        float midY = tower.GetTowerHeight() * 0.5f;
        Vector3 target = tower.transform.position + Vector3.up * (midY + heightOffset);
        ApplyPosition(target, true);
    }

    public void FocusOnLayer(int layerIndex)
    {
        float targetY = tower.GetLayerWorldY(layerIndex);
        Vector3 target = tower.transform.position + Vector3.up * (targetY + heightOffset);
        ApplyPosition(target, false);
    }

    private void ApplyPosition(Vector3 lookTarget, bool instant)
    {
        Quaternion rotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);
        Vector3 offset = rotation * (Vector3.back * distance);
        Vector3 targetPos = lookTarget + offset;

        if (instant)
        {
            transform.position = targetPos;
            transform.LookAt(lookTarget);
            currentLookTarget = lookTarget;
            return;
        }

        moveTween?.Kill();
        lookTween?.Kill();

        moveTween = transform.DOMove(targetPos, followDuration).SetEase(Ease.OutCubic);
        lookTween = DOTween.To(
            () => currentLookTarget,
            x =>
            {
                currentLookTarget = x;
                transform.LookAt(currentLookTarget);
            },
            lookTarget,
            followDuration
        ).SetEase(Ease.OutCubic);
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
        lookTween?.Kill();
    }
}
