using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class AdventureManager : MonoBehaviour, ILevelManager
{
    // This is just a place holder to see if portals work
    // In main menu player will not be spawned
    public Vector3 playerStartingPos = new(3,3,3);
    [SerializeField] private GameObject _player;
    [SerializeField] private CameraManager _cameraManager;

    [Range(10, 300)]
    public int width = 10, height = 10;
    [Range(1, 20)]
    public int iterations = 0;

    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private MeshFilter _mesh;
    private MapGenerator _generator = new MapGenerator();
    private MapGenerator.Tile[,] _mapGrid;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = GetComponent<MeshFilter>();
    }
    private void Start()
    {
        _mapGrid = _generator.GetMap(width, height, iterations);
        StringBuilder sb = new StringBuilder();
        for(int i=0; i< _mapGrid.GetLength(0); i++)
        {
            for(int j=0; j< _mapGrid.GetLength(1); j++)
            {
                sb.Append(_mapGrid[i,j]);
                sb.Append(' ');				   
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());

        GenerateMeshMap(_mapGrid);

        Instantiate(_player, playerStartingPos, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _cameraManager.Target = player.transform;
    }

    private void Update()
    {
        
    }

    private void GenerateMeshMap(MapGenerator.Tile[,] map)
    {
        Mesh mesh = new();
        int quadWidth = 3;
        
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uv = new();

        int mapWidth = map.GetLength(0), mapHeight = map.GetLength(0);
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                int index = x * mapHeight + y;
                MapGenerator.Tile tile = map[x, y];

                Vector3 p0 = new(quadWidth * x,     0, quadWidth * y);
                Vector3 p1 = new(quadWidth * x,     0, quadWidth * (y+1));
                Vector3 p2 = new(quadWidth * (x+1), 0, quadWidth * (y+1));
                Vector3 p3 = new(quadWidth * (x+1), 0, quadWidth * y);

                Vector3 p4 = new(quadWidth * x,     quadWidth, quadWidth * y);
                Vector3 p5 = new(quadWidth * x,     quadWidth, quadWidth * (y+1));
                Vector3 p6 = new(quadWidth * (x+1), quadWidth, quadWidth * (y+1));
                Vector3 p7 = new(quadWidth * (x+1), quadWidth, quadWidth * y);

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                vertices.Add(p4);
                vertices.Add(p5);
                vertices.Add(p6);
                vertices.Add(p7);

                uv.Add(new (0, 0));
                uv.Add(new (0, 1));
                uv.Add(new (1, 1));
                uv.Add(new (1, 0));

                if(tile == MapGenerator.Tile.FLOOR)
                {

                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 1);
                    triangles.Add(index * 8 + 2);
                    
                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 2);
                    triangles.Add(index * 8 + 3);
                }
                if(tile == MapGenerator.Tile.WALL)
                {

                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 1);
                    triangles.Add(index * 8 + 5);
                    
                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 5);
                    triangles.Add(index * 8 + 4);

                    triangles.Add(index * 8 + 5);
                    triangles.Add(index * 8 + 1);
                    triangles.Add(index * 8 + 2);

                    triangles.Add(index * 8 + 5);
                    triangles.Add(index * 8 + 2);
                    triangles.Add(index * 8 + 6);

                    triangles.Add(index * 8 + 7);
                    triangles.Add(index * 8 + 6);
                    triangles.Add(index * 8 + 2);

                    triangles.Add(index * 8 + 7);
                    triangles.Add(index * 8 + 2);
                    triangles.Add(index * 8 + 3);
                    
                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 4);
                    triangles.Add(index * 8 + 7);

                    triangles.Add(index * 8 + 0);
                    triangles.Add(index * 8 + 7);
                    triangles.Add(index * 8 + 3);

                    triangles.Add(index * 8 + 4);
                    triangles.Add(index * 8 + 5);
                    triangles.Add(index * 8 + 6);

                    triangles.Add(index * 8 + 4);
                    triangles.Add(index * 8 + 6);
                    triangles.Add(index * 8 + 7);
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uv);

        _mesh.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }
}