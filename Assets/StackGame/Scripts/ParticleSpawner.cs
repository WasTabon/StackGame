using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;

    private void Awake()
    {
        SetupParticleSystem();
    }

    private void SetupParticleSystem()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
            ps = gameObject.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 1.2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.12f);
        main.gravityModifier = 1.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 200;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.4f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.3f));

        psRenderer = GetComponent<ParticleSystemRenderer>();
        psRenderer.renderMode = ParticleSystemRenderMode.Mesh;
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        psRenderer.mesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempCube);
        psRenderer.material = new Material(Shader.Find("Standard"));
    }

    public void SpawnAt(Vector3 worldPos, Color color, int count = 20)
    {
        transform.position = worldPos;

        var main = ps.main;
        main.startColor = color;

        ps.Emit(count);
    }

    public void SpawnForLayer(BlockLayer layer)
    {
        Vector3 pos = layer.transform.position;
        for (int i = 0; i < 4; i++)
        {
            Color c = GameColors.FromIndex(layer.colorIndices[i * 2]);
            SpawnAt(pos, c, 8);
        }
    }
}
