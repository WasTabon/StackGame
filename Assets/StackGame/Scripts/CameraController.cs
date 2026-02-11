using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Tower tower;
    public float distance = 4f;
    public float heightOffset = 0.5f;
    public float pitchAngle = 30f;
    public float yawAngle = 35f;

    private void Start()
    {
        if (tower != null && tower.layers.Count > 0)
            FocusOnTowerCenter();
    }

    public void FocusOnTowerCenter()
    {
        float midY = tower.GetTowerHeight() * 0.5f;
        Vector3 target = tower.transform.position + Vector3.up * (midY + heightOffset);

        Quaternion rotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);
        Vector3 offset = rotation * (Vector3.back * distance);

        transform.position = target + offset;
        transform.LookAt(target);
    }
}
