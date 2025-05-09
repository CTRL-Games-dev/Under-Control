using System;
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
        // float[,] th = new float[gridWidth+1, gridHeight+1];
        // for(int x = 0; x < gridWidth; x++)
        // {
        //     for(int y = 0; y < gridHeight; y++) {
        //         th[x,y] = UnityEngine.Random.Range(-0.1f, -0.05f);
        //     }
        // }

        int parts = 5;

        int gridWidthPart = (int)Math.Ceiling((float)gridWidth / parts);
        int gridHeightPart = (int)Math.Ceiling((float)gridHeight / parts);

        int quadIndex = 0; // This tracks how many quads we've created

        for (int px = 0; px < parts; px++)
        {
            for (int py = 0; py < parts; py++)
            {
                int startX = px * gridWidthPart;
                int startY = py * gridHeightPart;

                int partWidth = Math.Min(gridWidthPart, gridWidth - startX);
                int partHeight = Math.Min(gridHeightPart, gridHeight - startY);

                int x = (int)(startX + topLeftCornerPosition.x);
                int y = (int)(startY + topLeftCornerPosition.y);

                Vector3 p0 = new Vector3(x, 0, y);
                Vector3 p1 = new Vector3(x, 0, y + partHeight);
                Vector3 p2 = new Vector3(x + partWidth, 0, y + partHeight);
                Vector3 p3 = new Vector3(x + partWidth, 0, y);

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                // Give each quad a full UV range
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));

                trianglesFloor.Add(quadIndex * 4 + 0);
                trianglesFloor.Add(quadIndex * 4 + 1);
                trianglesFloor.Add(quadIndex * 4 + 2);
                trianglesFloor.Add(quadIndex * 4 + 0);
                trianglesFloor.Add(quadIndex * 4 + 2);
                trianglesFloor.Add(quadIndex * 4 + 3);

                quadIndex++;
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