using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputController : MonoBehaviour
{
    public Tower tower;
    public CameraController cameraController;
    public StackChecker stackChecker;
    public Text layerIndicatorText;

    private int selectedIndex = 0;
    private bool isRotating = false;
    private bool inputLocked = false;
    private float rotateDuration = 0.25f;

    private void Start()
    {
        SelectLayer(0);
    }

    private void Update()
    {
        if (inputLocked) return;
        if (tower.layers.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelection(1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RotateSelected(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            RotateSelected(1);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            OnConfirmPressed();
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
    }

    public void MoveSelection(int direction)
    {
        if (inputLocked) return;
        if (tower.layers.Count == 0) return;

        int newIndex = selectedIndex + direction;
        newIndex = Mathf.Clamp(newIndex, 0, tower.layers.Count - 1);

        if (newIndex != selectedIndex)
            SelectLayer(newIndex);
    }

    public void RotateSelected(int direction)
    {
        if (isRotating || inputLocked) return;
        if (tower.layers.Count == 0) return;

        BlockLayer layer = tower.layers[selectedIndex];
        isRotating = true;

        float targetAngle = direction > 0 ? -90f : 90f;

        layer.transform.DOLocalRotate(new Vector3(0f, targetAngle, 0f), rotateDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutBack, 1.2f)
            .OnComplete(() =>
            {
                //layer.ApplyRotation(direction);

                float y = layer.transform.localEulerAngles.y;
                y = Mathf.Round(y / 90f) * 90f;
                layer.transform.localEulerAngles = new Vector3(0f, y, 0f);

                isRotating = false;
            });
    }

    public void OnConfirmPressed()
    {
        if (inputLocked) return;
        Debug.Assert(stackChecker != null, "StackChecker not assigned on InputController!");
        stackChecker.CheckAndResolve();
    }

    public void RefreshSelection()
    {
        if (tower.layers.Count == 0) return;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, tower.layers.Count - 1);
        SelectLayer(selectedIndex);
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
