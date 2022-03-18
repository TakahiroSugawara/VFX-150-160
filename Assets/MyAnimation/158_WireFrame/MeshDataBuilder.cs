// Based on: https://catlikecoding.com/unity/tutorials/advanced-rendering/flat-and-wireframe-shading/

using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(MeshFilter))]
public class MeshDataBuilder : MonoBehaviour
{

    private void Reset()
    {
        GenerateMeshData();
    }

    /// <summary>
    /// We will assign a color to each Vertex in a Triangle on the object's mesh
    /// </summary>
    void GenerateMeshData()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        SplitMesh(mesh);
        SetVertexColors(mesh);

        this.GetComponent<VisualEffect>().SetMesh("_Mesh", mesh);
}
    /// <summary>
    /// 
    /// このアプローチでは、共有頂点がないことを確認する必要があります
    /// メッシュ上で、この方法を使用してメッシュを分割するのはそのためです。
    /// これにより、頂点の数が増えるため、最適化が不十分になります。
    /// 
    /// For this approach, we need to make sure there are not shared vertices
    /// on the mesh, that's why we use this method to split the mesh. 
    /// This will increase the number of vertices, so less optimized.
    /// 
    /// </summary>
    /// <param name="mesh"></param>
    void SplitMesh(Mesh mesh)
    {

        int[] triangles = mesh.triangles;        //メッシュ内のすべての三角ポリゴンの頂点の配列
        Vector3[] verts = mesh.vertices;         //メッシュ内のすべての頂点
        Vector3[] normals = mesh.normals;        //メッシュ内のすべての法線
        Vector2[] uvs = mesh.uv;                 //メッシュ内のすべてのuv

        Vector3[] newVerts;
        Vector3[] newNormals;
        Vector2[] newUvs;

        int n = triangles.Length;
        newVerts = new Vector3[n];
        newNormals = new Vector3[n];
        newUvs = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            newVerts[i] = verts[triangles[i]];
            newNormals[i] = normals[triangles[i]];
            if (uvs.Length > 0)
            {
                newUvs[i] = uvs[triangles[i]];
            }
            triangles[i] = i;
        }

        mesh.vertices = newVerts;
        mesh.normals = newNormals;
        mesh.uv = newUvs;
        mesh.triangles = triangles;
    }

    /// <summary>
    /// We paint the vertex color
    /// </summary>
    /// <param name="mesh"></param>
    void SetVertexColors(Mesh mesh)
    {
        Color[] colorCoords = new[]
        {
            new Color(1, 0, 0),
            new Color(0, 1, 0),
            new Color(0, 0, 1),
        };

        Color32[] vertexColors = new Color32[mesh.vertices.Length];

        for (int i = 0; i < vertexColors.Length; i += 3)
        {
            vertexColors[i] = colorCoords[0];
            vertexColors[i + 1] = colorCoords[1];
            vertexColors[i + 2] = colorCoords[2];
        }

        mesh.colors32 = vertexColors;
    }
}