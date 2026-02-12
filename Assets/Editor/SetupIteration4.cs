using UnityEngine;
using UnityEditor;

public class SetupIteration4 : Editor
{
    [MenuItem("STACK/Setup Stack Mechanic (Iteration 4)")]
    public static void Setup()
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();
        Debug.Assert(tower != null, "Tower not found! Run Iteration 1 setup first.");

        InputController input = Object.FindFirstObjectByType<InputController>();
        Debug.Assert(input != null, "InputController not found! Run Iteration 2 setup first.");

        StackChecker checker = SetupStackChecker(tower, input);

        input.stackChecker = checker;
        EditorUtility.SetDirty(input);

        Debug.Log("[Iteration 4] Stack mechanic setup complete.");
    }

    private static StackChecker SetupStackChecker(Tower tower, InputController input)
    {
        StackChecker checker = Object.FindFirstObjectByType<StackChecker>();
        if (checker == null)
        {
            GameObject obj = new GameObject("StackChecker");
            checker = obj.AddComponent<StackChecker>();
            Undo.RegisterCreatedObjectUndo(obj, "Create StackChecker");
        }

        checker.tower = tower;
        checker.inputController = input;
        EditorUtility.SetDirty(checker);
        return checker;
    }
}
