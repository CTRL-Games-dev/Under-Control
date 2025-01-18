using System.Collections.Generic;
using System.Text;

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

    public readonly Vector2 GetCenter(float tileWidth)
    {
        return new Vector2(X + 0.5f, Y + 0.5f) * tileWidth;
    }
}

public class WorldMap
{
    public TileType[,] Tiles;
    public Vector2 Size;
    public List<Location> Locations = new();
    public readonly float TileWidth;
    public Vector3 SpawnLocation = new(0,0);

    public Material Grass, Walls;
    public WorldMap(TileType[,] tiles, Vector2 size, float tileWidth) {
        Tiles = tiles;
        Size = size;
        TileWidth = tileWidth;

        SpawnLocation = new Vector3(size[0], 1, size[1]) /2*tileWidth;
    }
    public WorldMap(TileType[,] tiles, int sizeX, int sizeY, float tileWidth) {
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
        GenerateSpawn();
        GenerateMesh(mr, mf, mc);
    }

    private void GenerateSpawn()
    {
        var mapCenter = Size / 2;

        for(int ix = -1; ix <= 1; ix++)
        {
            for(int iy = -1; iy <= 1; iy++)
            {
                Tiles[(int)mapCenter.x+ix, (int)mapCenter.y+iy] = TileType.FLOOR;
            }
        }
    }

    private void GenerateMesh(MeshRenderer mr, MeshFilter mf, MeshCollider mc)
    {
        Mesh newMesh = new();

        newMesh.subMeshCount = 2;
        
        List<Vector3> vertices = new();
        List<int> trianglesFloor = new();
        List<int> trianglesWalls = new();
        List<Vector2> uv = new();

        // var grassTexture = Resources.Load<Material>("Assets/!/Materials/Placeholders/Adventure Grass.mat");
        // var wallTexture = Resources.Load<Material>("Assets/!/Materials/Placeholders/Adventure Walls.mat");

        int mapWidth = Tiles.GetLength(0), mapHeight = Tiles.GetLength(1);
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++) {
                int index = x * ((int)Size[0]) + y;
                var tile = Tiles[x, y];

                Vector3 p0 = new(TileWidth * x,     0, TileWidth * y);
                Vector3 p1 = new(TileWidth * x,     0, TileWidth * (y+1));
                Vector3 p2 = new(TileWidth * (x+1), 0, TileWidth * (y+1));
                Vector3 p3 = new(TileWidth * (x+1), 0, TileWidth * y);

                Vector3 p4 = new(TileWidth * x,     TileWidth, TileWidth * y);
                Vector3 p5 = new(TileWidth * x,     TileWidth, TileWidth * (y+1));
                Vector3 p6 = new(TileWidth * (x+1), TileWidth, TileWidth * (y+1));
                Vector3 p7 = new(TileWidth * (x+1), TileWidth, TileWidth * y);

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                vertices.Add(p4);
                vertices.Add(p5);
                vertices.Add(p6);
                vertices.Add(p7);

                uv.Add(new (0, 0));
                uv.Add(new (1, 0));
                uv.Add(new (1, 1));
                uv.Add(new (0, 1));

                uv.Add(new (0, 0));
                uv.Add(new (1, 0));
                uv.Add(new (1, 1));
                uv.Add(new (0, 1));

                if(tile == TileType.FLOOR)
                {

                    trianglesFloor.Add(index * 8 + 0);
                    trianglesFloor.Add(index * 8 + 1);
                    trianglesFloor.Add(index * 8 + 2);
                    
                    trianglesFloor.Add(index * 8 + 0);
                    trianglesFloor.Add(index * 8 + 2);
                    trianglesFloor.Add(index * 8 + 3);
                }
                if(tile == TileType.WALL)
                {

                    trianglesWalls.Add(index * 8 + 0);
                    trianglesWalls.Add(index * 8 + 1);
                    trianglesWalls.Add(index * 8 + 5);
                    
                    trianglesWalls.Add(index * 8 + 0);
                    trianglesWalls.Add(index * 8 + 5);
                    trianglesWalls.Add(index * 8 + 4);

                    trianglesWalls.Add(index * 8 + 5);
                    trianglesWalls.Add(index * 8 + 1);
                    trianglesWalls.Add(index * 8 + 2);

                    trianglesWalls.Add(index * 8 + 5);
                    trianglesWalls.Add(index * 8 + 2);
                    trianglesWalls.Add(index * 8 + 6);

                    trianglesWalls.Add(index * 8 + 7);
                    trianglesWalls.Add(index * 8 + 6);
                    trianglesWalls.Add(index * 8 + 2);

                    trianglesWalls.Add(index * 8 + 7);
                    trianglesWalls.Add(index * 8 + 2);
                    trianglesWalls.Add(index * 8 + 3);
                    
                    trianglesWalls.Add(index * 8 + 0);
                    trianglesWalls.Add(index * 8 + 4);
                    trianglesWalls.Add(index * 8 + 7);

                    trianglesWalls.Add(index * 8 + 0);
                    trianglesWalls.Add(index * 8 + 7);
                    trianglesWalls.Add(index * 8 + 3);

                    trianglesWalls.Add(index * 8 + 4);
                    trianglesWalls.Add(index * 8 + 5);
                    trianglesWalls.Add(index * 8 + 6);

                    trianglesWalls.Add(index * 8 + 4);
                    trianglesWalls.Add(index * 8 + 6);
                    trianglesWalls.Add(index * 8 + 7);
                }
            }
        }

        newMesh.SetVertices(vertices);
        newMesh.SetTriangles(trianglesFloor, 0);
        newMesh.SetTriangles(trianglesWalls, 1);
        newMesh.SetUVs(0, uv);

        mf.mesh = newMesh;
        mr.materials = new Material[] {Grass, Walls};
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

public interface Location
{
    public void GenerateLocation(WorldMap map);
}
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
