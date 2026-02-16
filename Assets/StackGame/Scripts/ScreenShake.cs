using UnityEngine;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
    public Camera cam;

    private Vector3 originalPos;
    private Tweener shakeTween;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    public void ShakeLight()
    {
        Shake(0.15f, 0.05f);
    }

    public void ShakeMedium()
    {
        Shake(0.25f, 0.12f);
    }

    public void ShakeHeavy()
    {
        Shake(0.35f, 0.2f);
    }

    public void ShakeForChain(int chainStep)
    {
        float duration = Mathf.Min(0.15f + chainStep * 0.1f, 0.5f);
        float strength = Mathf.Min(0.05f + chainStep * 0.08f, 0.35f);
        Shake(duration, strength);
    }

    private void Shake(float duration, float strength)
    {
        if (cam == null) return;

        shakeTween?.Kill(true);
        shakeTween = cam.transform.DOShakePosition(duration, strength, 20, 90f, false, true, ShakeRandomnessMode.Harmonic)
            .SetRelative(true);
    }

    private void OnDestroy()
    {
        shakeTween?.Kill();
    }
}
