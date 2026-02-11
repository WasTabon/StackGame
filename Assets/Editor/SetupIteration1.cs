using UnityEngine;
using UnityEditor;

public class SetupIteration1 : Editor
{
    [MenuItem("STACK/Setup Gameplay Scene (Iteration 1)")]
    public static void Setup()
    {
        Material blockMat = SetupMaterial();
        Tower tower = SetupTower(blockMat);
        tower.SpawnInitialLayers();
        CameraController cam = SetupCamera(tower);
        cam.FocusOnTowerCenter();
        SetupLighting();

        Debug.Log("[Iteration 1] Gameplay scene setup complete.");
    }

    private static Material SetupMaterial()
    {
        string matPath = "Assets/STACK/Materials/BlockVertexColor.mat";

        if (!AssetDatabase.IsValidFolder("Assets/STACK/Materials"))
            AssetDatabase.CreateFolder("Assets/STACK", "Materials");

        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            Shader shader = Shader.Find("STACK/VertexColor");
            Debug.Assert(shader != null, "STACK/VertexColor shader not found! Make sure the shader file exists.");
            mat = new Material(shader);
            mat.name = "BlockVertexColor";
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
        }

        return mat;
    }

    private static Tower SetupTower(Material blockMat)
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();

        if (tower == null)
        {
            GameObject towerObj = new GameObject("Tower");
            towerObj.transform.position = Vector3.zero;
            tower = towerObj.AddComponent<Tower>();
            Undo.RegisterCreatedObjectUndo(towerObj, "Create Tower");
        }
        else
        {
            tower.ClearLayers();
        }

        tower.blockMaterial = blockMat;
        tower.initialLayerCount = 6;
        tower.layerSpacing = 0.5f;

        EditorUtility.SetDirty(tower);
        return tower;
    }

    private static CameraController SetupCamera(Tower tower)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
            Undo.RegisterCreatedObjectUndo(camObj, "Create Camera");
        }

        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = GameColors.Background;
        mainCam.fieldOfView = 45f;

        CameraController controller = mainCam.GetComponent<CameraController>();
        if (controller == null)
            controller = mainCam.gameObject.AddComponent<CameraController>();

        controller.tower = tower;
        controller.distance = 4.5f;
        controller.heightOffset = 0.3f;
        controller.pitchAngle = 25f;
        controller.yawAngle = 35f;

        EditorUtility.SetDirty(mainCam.gameObject);
        return controller;
    }

    private static void SetupLighting()
    {
        Light dirLight = null;
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional)
            {
                dirLight = l;
                break;
            }
        }

        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
            Undo.RegisterCreatedObjectUndo(lightObj, "Create Directional Light");
        }

        dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        dirLight.color = new Color(0.7f, 0.75f, 0.9f, 1f);
        dirLight.intensity = 0.8f;
        dirLight.shadows = LightShadows.Soft;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.12f, 0.13f, 0.22f, 1f);

        EditorUtility.SetDirty(dirLight.gameObject);
    }
}
