using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputController : MonoBehaviour
{
    public Tower tower;
    public CameraController cameraController;
    public Text layerIndicatorText;

    private int selectedIndex = 0;
    private bool isRotating = false;
    private float rotateDuration = 0.25f;

    private void Start()
    {
        SelectLayer(0);
    }

    private void Update()
    {
        if (tower.layers.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelection(1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RotateSelected(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            RotateSelected(1);
    }

    public void MoveSelection(int direction)
    {
        if (tower.layers.Count == 0) return;

        int newIndex = selectedIndex + direction;
        newIndex = Mathf.Clamp(newIndex, 0, tower.layers.Count - 1);

        if (newIndex != selectedIndex)
            SelectLayer(newIndex);
    }

    public void RotateSelected(int direction)
    {
        if (isRotating) return;
        if (tower.layers.Count == 0) return;

        BlockLayer layer = tower.layers[selectedIndex];
        isRotating = true;

        float targetAngle = direction > 0 ? -90f : 90f;
        Vector3 startRot = layer.transform.localEulerAngles;
        float startY = startRot.y;
        float endY = startY + targetAngle;

        layer.transform.DOLocalRotate(new Vector3(0f, endY, 0f), rotateDuration, RotateMode.Fast)
            .SetEase(Ease.OutBack, 1.2f)
            .OnComplete(() =>
            {
                if (direction > 0)
                    layer.RotateColorsCW();
                else
                    layer.RotateColorsCCW();

                layer.transform.localEulerAngles = new Vector3(0f, Mathf.Round(layer.transform.localEulerAngles.y / 90f) * 90f, 0f);
                isRotating = false;
            });
    }

    public void OnConfirmPressed()
    {
    }

    private void SelectLayer(int index)
    {
        if (selectedIndex >= 0 && selectedIndex < tower.layers.Count)
            tower.layers[selectedIndex].SetHighlight(false);

        selectedIndex = index;
        tower.layers[selectedIndex].SetHighlight(true);
        cameraController.FocusOnLayer(selectedIndex);
        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        if (layerIndicatorText != null)
            layerIndicatorText.text = "Layer " + (selectedIndex + 1) + " / " + tower.layers.Count;
    }

    public void OnMoveUp() => MoveSelection(1);
    public void OnMoveDown() => MoveSelection(-1);
    public void OnRotateLeft() => RotateSelected(-1);
    public void OnRotateRight() => RotateSelected(1);
}
