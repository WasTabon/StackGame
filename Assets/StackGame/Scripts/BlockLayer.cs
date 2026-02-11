using UnityEngine;
using System.Collections.Generic;

public class BlockLayer : MonoBehaviour
{
    public int[] colorIndices = new int[8];
    public float blockWidth = 1f;
    public float blockHeight = 0.4f;
    public float blockDepth = 1f;
    public float stripWidth = 0.03f;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;
    private List<Vector3> normals;

    private int colorStartIndex;

    public void Initialize(int[] newColors, Material material)
    {
        System.Array.Copy(newColors, colorIndices, 8);

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = material;
        GenerateMesh();
    }

    public int GetColorIndex(int side, int half)
    {
        return colorIndices[side * 2 + half];
    }

    public void RotateColorsCW()
    {
        int[] newColors = new int[8];
        for (int h = 0; h < 2; h++)
        {
            newColors[0 * 2 + h] = colorIndices[3 * 2 + h];
            newColors[1 * 2 + h] = colorIndices[0 * 2 + h];
            newColors[2 * 2 + h] = colorIndices[1 * 2 + h];
            newColors[3 * 2 + h] = colorIndices[2 * 2 + h];
        }
        System.Array.Copy(newColors, colorIndices, 8);
        UpdateVertexColors();
    }

    public void RotateColorsCCW()
    {
        int[] newColors = new int[8];
        for (int h = 0; h < 2; h++)
        {
            newColors[0 * 2 + h] = colorIndices[1 * 2 + h];
            newColors[1 * 2 + h] = colorIndices[2 * 2 + h];
            newColors[2 * 2 + h] = colorIndices[3 * 2 + h];
            newColors[3 * 2 + h] = colorIndices[0 * 2 + h];
        }
        System.Array.Copy(newColors, colorIndices, 8);
        UpdateVertexColors();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "BlockLayerMesh";

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        normals = new List<Vector3>();

        float hw = blockWidth * 0.5f;
        float hh = blockHeight * 0.5f;
        float hd = blockDepth * 0.5f;
        float totalW = blockWidth;
        float halfW = (totalW - stripWidth) * 0.5f;
        float height = blockHeight;

        Vector3[] sideOrigins = new Vector3[]
        {
            new Vector3(-hw, -hh, hd),
            new Vector3(hw, -hh, hd),
            new Vector3(hw, -hh, -hd),
            new Vector3(-hw, -hh, -hd)
        };

        Vector3[] sideRights = new Vector3[]
        {
            Vector3.right,
            Vector3.back,
            Vector3.left,
            Vector3.forward
        };

        Vector3[] sideNormals = new Vector3[]
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

        colorStartIndex = 0;

        for (int side = 0; side < 4; side++)
        {
            Vector3 origin = sideOrigins[side];
            Vector3 right = sideRights[side];
            Vector3 normal = sideNormals[side];

            Color leftColor = GameColors.FromIndex(colorIndices[side * 2 + 0]);
            Color rightColor = GameColors.FromIndex(colorIndices[side * 2 + 1]);

            if (side == 0)
                colorStartIndex = vertices.Count;

            AddQuad(origin, right, halfW, height, normal, leftColor);
            AddQuad(origin + right * halfW, right, stripWidth, height, normal, GameColors.DarkStrip);
            AddQuad(origin + right * (halfW + stripWidth), right, halfW, height, normal, rightColor);
        }

        AddQuad(
            new Vector3(-hw, hh, -hd),
            Vector3.right, totalW,
            new Vector3(-hw, hh, hd),
            Vector3.up, GameColors.DarkBase
        );

        AddQuad(
            new Vector3(-hw, -hh, hd),
            Vector3.right, totalW,
            new Vector3(-hw, -hh, -hd),
            Vector3.down, GameColors.DarkBase
        );

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.SetNormals(normals);

        meshFilter.mesh = mesh;
    }

    private void AddQuad(Vector3 bottomLeft, Vector3 rightDir, float width, float height, Vector3 normal, Color color)
    {
        int idx = vertices.Count;

        vertices.Add(bottomLeft);
        vertices.Add(bottomLeft + rightDir * width);
        vertices.Add(bottomLeft + rightDir * width + Vector3.up * height);
        vertices.Add(bottomLeft + Vector3.up * height);

        triangles.Add(idx);
        triangles.Add(idx + 1);
        triangles.Add(idx + 2);
        triangles.Add(idx);
        triangles.Add(idx + 2);
        triangles.Add(idx + 3);

        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(normal);
        }
    }

    private void AddQuad(Vector3 corner1, Vector3 rightDir, float width, Vector3 corner2, Vector3 normal, Color color)
    {
        int idx = vertices.Count;

        Vector3 bl = corner1;
        Vector3 br = corner1 + rightDir * width;
        Vector3 tl = corner2;
        Vector3 tr = corner2 + rightDir * width;

        vertices.Add(bl);
        vertices.Add(br);
        vertices.Add(tr);
        vertices.Add(tl);

        triangles.Add(idx);
        triangles.Add(idx + 2);
        triangles.Add(idx + 1);
        triangles.Add(idx);
        triangles.Add(idx + 3);
        triangles.Add(idx + 2);

        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(normal);
        }
    }

    public void UpdateVertexColors()
    {
        if (mesh == null) return;

        Color[] meshColors = mesh.colors;

        float totalW = blockWidth;
        float halfW = (totalW - stripWidth) * 0.5f;

        int idx = colorStartIndex;

        for (int side = 0; side < 4; side++)
        {
            Color leftColor = GameColors.FromIndex(colorIndices[side * 2 + 0]);
            Color rightColor = GameColors.FromIndex(colorIndices[side * 2 + 1]);

            for (int v = 0; v < 4; v++)
                meshColors[idx + v] = leftColor;
            idx += 4;

            for (int v = 0; v < 4; v++)
                meshColors[idx + v] = GameColors.DarkStrip;
            idx += 4;

            for (int v = 0; v < 4; v++)
                meshColors[idx + v] = rightColor;
            idx += 4;
        }

        mesh.colors = meshColors;
    }
}
