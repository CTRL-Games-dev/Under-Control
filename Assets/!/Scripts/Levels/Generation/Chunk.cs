using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;
    private GameObject _terrainHolder;
    public void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    public void GenerateChunkMesh(Vector2 topLeftCornerPosition, int gridWidth, int gridHeight)
    {
        Mesh newMesh = new();
        
        List<Vector3> vertices = new();
        List<int> trianglesFloor = new();
        List<Vector2> uv = new();

        // Generate terrain height
        float[,] th = new float[gridWidth+1, gridHeight+1];
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++) {
                th[x,y] = UnityEngine.Random.Range(-0.1f, -0.05f);
            }
        }

        for(int ix = 0; ix < gridWidth; ix++)
        {
            for(int iy = 0; iy < gridHeight; iy++) {
                int index = iy * gridHeight + iy;

                int x = (int)(ix + topLeftCornerPosition.x);
                int y = (int)(iy + topLeftCornerPosition.y);

                Vector3 p0 = new Vector3(x,     th[x,y],  y);
                Vector3 p1 = new Vector3(x,     th[x,y+1],  (y+1));
                Vector3 p2 = new Vector3((x+1), th[x+1,y+1],  (y+1));
                Vector3 p3 = new Vector3((x+1), th[x+1,y], y);

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                uv.Add(new (0, 0));
                uv.Add(new (0, 1));
                uv.Add(new (1, 1));
                uv.Add(new (1, 0));

                trianglesFloor.Add(index * 4 + 0);
                trianglesFloor.Add(index * 4 + 1);
                trianglesFloor.Add(index * 4 + 2);
                
                trianglesFloor.Add(index * 4 + 0);
                trianglesFloor.Add(index * 4 + 2);
                trianglesFloor.Add(index * 4 + 3);
            }
        }


        newMesh.SetVertices(vertices);
        newMesh.SetTriangles(trianglesFloor, 0);
        newMesh.SetUVs(0, uv);
        newMesh.RecalculateNormals();
        // newMesh.RecalculateBounds();
        newMesh.RecalculateTangents();
        newMesh.Optimize();

        // Get material
        Material dirt = Resources.Load<Material>("Materials/Forest/Grass/Grass");

        _meshFilter.mesh = newMesh;
        _meshRenderer.materials = new Material[] {dirt};
        _meshCollider.sharedMesh = newMesh;
    }
}