using UnityEngine;
using UnityEditor;

public class SetupIteration1 : Editor
{
    [MenuItem("STACK/Setup Gameplay Scene (Iteration 1)")]
    public static void Setup()
    {
        Tower tower = SetupTower();
        tower.SpawnInitialLayers();

        SetupCamera(tower);
        SetupLighting();

        Debug.Log("[Iteration 1] Gameplay scene setup complete. " + tower.layers.Count + " layers spawned.");
    }

    private static Tower SetupTower()
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();
        if (tower != null)
        {
            tower.ClearLayers();
        }
        else
        {
            GameObject obj = new GameObject("Tower");
            tower = obj.AddComponent<Tower>();
            Undo.RegisterCreatedObjectUndo(obj, "Create Tower");
        }

        EditorUtility.SetDirty(tower);
        return tower;
    }

    private static void SetupCamera(Tower tower)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
            Undo.RegisterCreatedObjectUndo(camObj, "Create Camera");
        }

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = GameColors.Background;
        cam.fieldOfView = 40f;

        float midY = tower.GetTowerHeight() * 0.5f;
        cam.transform.position = new Vector3(2f, midY + 0.5f, 2f);
        cam.transform.LookAt(new Vector3(0f, midY, 0f));

        CameraController cc = cam.GetComponent<CameraController>();
        if (cc == null)
            cc = cam.gameObject.AddComponent<CameraController>();
        cc.tower = tower;

        EditorUtility.SetDirty(cam);
        EditorUtility.SetDirty(cc);
    }

    private static void SetupLighting()
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in lights)
            DestroyImmediate(l.gameObject);

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.9f, 0.9f, 1f, 1f);
        light.intensity = 1.0f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Undo.RegisterCreatedObjectUndo(lightObj, "Create Light");
        EditorUtility.SetDirty(light);
    }
}
