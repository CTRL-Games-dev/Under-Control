using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Internal;
using UnityEngine;

public enum TileType
{
    WALL,
    FLOOR,
}

public struct Tile
{
    public int X;
    public int Y;
    public TileType Type;
    public GameObject Instance;
    public readonly Vector2 GetCenter(float tileWidth)
    {
        return new Vector2(X + 0.5f, Y + 0.5f) * tileWidth;
    }
}

public class WorldMap
{
    public Tile[,] Tiles;
    public Vector2 Size;
    public List<Location> Locations = new();
    public readonly float TileWidth;
    public Vector3 SpawnLocation = new(0,0);
    public Material Grass, Walls;
    public List<IBiome> biomes = new();
    public WorldMap(Tile[,] tiles, Vector2 size, float tileWidth) {
        Tiles = tiles;
        Size = size;
        TileWidth = tileWidth;

        SpawnLocation = new Vector3(size[0], 1, size[1]) /2*tileWidth;
    }
    public WorldMap(Tile[,] tiles, int sizeX, int sizeY, float tileWidth) {
        Tiles = tiles;
        Size = new Vector3(sizeX, 1, sizeY);
        TileWidth = tileWidth;

        SpawnLocation = new Vector3(sizeX, 1, sizeY)/2*tileWidth;
    }

    public void Generate(MeshRenderer mr, MeshFilter mf, MeshCollider mc)
    {

        foreach(var l in Locations)
        {
            l.GenerateLocation(this);
        }
        //GenerateSpawn();
        GenerateMesh(mr, mf, mc);
    }

    // private void GenerateSpawn()
    // {
    //     var mapCenter = Size / 2;

    //     for(int ix = -1; ix <= 1; ix++)
    //     {
    //         for(int iy = -1; iy <= 1; iy++)
    //         {
    //             Tiles[(int)mapCenter.x+ix, (int)mapCenter.y+iy] = TileType.FLOOR;
    //         }
    //     }
    // }

    private void GenerateMesh(MeshRenderer mr, MeshFilter mf, MeshCollider mc)
    {
        Mesh newMesh = new();
        
        List<Vector3> vertices = new();
        List<int> trianglesFloor = new();
        List<Vector2> uv = new();

        // var grassTexture = Resources.Load<Material>("Assets/!/Materials/Placeholders/Adventure Grass.mat");
        // var wallTexture = Resources.Load<Material>("Assets/!/Materials/Placeholders/Adventure Walls.mat");

        // wall prefab should have dimensions: 1x1
        var wallPrefabs = new List<GameObject>();
        wallPrefabs.Add(Resources.Load<GameObject>("Prefabs/Forest/ForestWall1"));
        wallPrefabs.Add(Resources.Load<GameObject>("Prefabs/Forest/ForestWall2"));
        wallPrefabs.Add(Resources.Load<GameObject>("Prefabs/Forest/ForestWall3"));
        wallPrefabs.Add(Resources.Load<GameObject>("Prefabs/Forest/ForestWall4"));

        var grassPrefabs = new List<GameObject>();
        grassPrefabs.Add(Resources.Load<GameObject>("Prefabs/Forest/Grass1"));


        int mapWidth = Tiles.GetLength(0), mapHeight = Tiles.GetLength(1);

        // Generate terrain height (Londek is not helpful)
        float[,] th = new float[mapWidth+1, mapHeight+1];
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++) {
                th[x,y] = Random.Range(-0.4f, 0.4f);
            }
        }

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++) {
                int index = x * ((int)Size[0]) + y;
                Tile tile = Tiles[x, y];

                // Add half of a tile, so it will be centered
                var pos = new Vector3(x * TileWidth + TileWidth/2, 0, y * TileWidth + TileWidth/2);

                Vector3 p0 = new(TileWidth * x,     th[x,y], TileWidth * y);
                Vector3 p1 = new(TileWidth * x,     th[x,y+1], TileWidth * (y+1));
                Vector3 p2 = new(TileWidth * (x+1), th[x+1,y+1], TileWidth * (y+1));
                Vector3 p3 = new(TileWidth * (x+1), th[x+1,y], TileWidth * y);

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                uv.Add(new (0, 0));
                uv.Add(new (1, 0));
                uv.Add(new (1, 1));
                uv.Add(new (0, 1));

                trianglesFloor.Add(index * 4 + 0);
                trianglesFloor.Add(index * 4 + 1);
                trianglesFloor.Add(index * 4 + 2);
                
                trianglesFloor.Add(index * 4 + 0);
                trianglesFloor.Add(index * 4 + 2);
                trianglesFloor.Add(index * 4 + 3);

                GameObject tilePrefab;
                if(tile.Type == TileType.WALL) tilePrefab = wallPrefabs[Random.Range(0, wallPrefabs.Count - 1)];
                else tilePrefab = grassPrefabs[Random.Range(0, grassPrefabs.Count - 1)];
                
                tile.Instance= GameObject.Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.Instance.transform.localScale += new Vector3(TileWidth, TileWidth, TileWidth);
            }
        }

        newMesh.SetVertices(vertices);
        newMesh.SetTriangles(trianglesFloor, 0);
        newMesh.SetUVs(0, uv);

        mf.mesh = newMesh;
        mr.materials = new Material[] {Grass};
        mc.sharedMesh = newMesh;
    }

    public List<Tile> GetMapAsList()
    {
        List<Tile> tilesList = new();

        for(int xi = 0; xi < Size[0]; xi++)
        {
            for(int yi = 0; yi < Size[1]; yi++)
            {
                var tile = new Tile {
                    X = xi,
                    Y = yi,
                    Type = Tiles[xi, yi]
                };
                tilesList.Add(tile);
            }
        }

        return tilesList;
    }
}

// public interface Location
// {
//     public void GenerateLocation(WorldMap map);
// }
public class MapGenerator
{
    public static WorldMap GetMap(int width, int height, int iterations, float tileWidth)
    {
        TileType[,] grid = GetNoise(width, height);

        for(int i = 0; i < iterations; i++)
        {
            TileType[,] temp_grid = CopyGrid(grid);
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    temp_grid[x,y] = CheckTile(grid, x, y);
                }
            }
            grid = temp_grid;
        }

        return new(grid, new(width, height), tileWidth);
    }

    private static TileType CheckTile(TileType[,] grid, int x, int y)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int wall_count = 0;
        for(int ix = -1; ix <= 1; ix++)
        {
            for(int iy = -1; iy <= 1; iy++)
            {
                if(iy == 0 && ix == 0)
                    continue;
                    

                int indexX = x + ix;
                int indexY = y + iy;

                if(indexX >= width || indexX < 0) { wall_count++; continue; }
                if(indexY >= height || indexY < 0) { wall_count++; continue; }
                if(grid[indexX, indexY] == TileType.WALL) { wall_count++; continue; }
            }
        }
        if(wall_count >= 4) 
            return TileType.WALL;
        else
            return TileType.FLOOR;
    }

    private static TileType[,] GetNoise(int width, int height)
    {
        TileType[,] tiles = new TileType[width, height];
        float density = 40;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float chance = Random.Range(0, 100);
                if(chance >= density)
                    tiles[x,y] = TileType.FLOOR;
                else
                    tiles[x,y] = TileType.WALL;
            }
        }

        return tiles;
    }

    private static TileType[,] CopyGrid(TileType[,] grid)
    {
        TileType[,] temp_grid = new TileType[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                temp_grid[i, j] = grid[i, j];
            }
        }
        return temp_grid;
    }
}


// LOCATIONS 

public class Forest : Location
{
    public void GenerateLocation(WorldMap map)
    {
        var mapCenter = map.Size / 2;

        for(int ix = -2; ix <= 1; ix++)
        {
            for(int iy = -2; iy <= 1; iy++)
            {
                map.Tiles[(int)mapCenter.x+ix, (int)mapCenter.y+iy] = TileType.FLOOR;
            }
        }

    }
}
