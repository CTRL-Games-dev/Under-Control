
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum CellType
{
    Empty,
    Forest,
}

public struct Grid
{
    public CellType[,] Cells;
    public Vector2 Dimensions;
    public int Padding;
    public Vector2 Offset;

    public int GetWidthCeil()
    {
        return (int)Math.Ceiling(Dimensions.x);
    }
    public int GetHeightCeil()
    {
        return (int)Math.Ceiling(Dimensions.y);
    }
}

public class WorldGenerator : MonoBehaviour {
    [SerializeField] private Chunk _chunkPrefab
    public void GenerateMap(Dimension type)
    {

        Grid grid;
        List<Location> locationPrefabs;

        switch(type)
        {
            case Dimension.FOREST: {
                (locationPrefabs, grid) = PlaceLocationsForest();
            } break;
            default: {
                Debug.LogError($"This type of dimension is not implemented! -> {type}");
                return;
            } break;
        }

        foreach(Location l in locationPrefabs)
        {
            for(int ix = 0; ix < (int)grid.Dimensions.x; ix++)
            {
                for (int iy = 0; iy < (int)grid.Dimensions.y; iy++)
                {
                    int x = (int)(ix + grid.Offset.x);
                    int y = (int)(iy + grid.Offset.y);

                    grid.Cells[x,y] = CellType.Forest;
                }
            }
        }

        DigOutPaths(locationPrefabs, ref grid);
        SpawnPrefabs();
        GenerateMesh();
    }


    #region Dimension types

    class LocationNode
    {
        public Location Location;
        public Vector2 Position;
        private List<Vector2> _directions;
        public LocationNode(Location l, Vector2 pos)
        {
            Location = l;
            Position = pos;

            _directions = new()
            {
                new(Position.x, Position.y + 1), // Up
                new(Position.x+1, Position.y), // Right
                new(Position.x, Position.y-1), // Down
                new(Position.x-1, Position.y), // Left
            };
        }

        public (bool, Vector2) HasFreeNeighbourSpace(List<LocationNode> allNodes)
        {

            var shuffledDirections = ShuffleList<Vector2>(_directions);

            foreach(var d in shuffledDirections) 
            {
                if(allNodes.Find(x => x.Position == d) == null) return (true, d);
            }

            return (false, Vector2.zero);
        }
        public static Vector2 GetRandomFreeNeighbourSpace(List<LocationNode> nodes)
        {
            List<LocationNode> randomOrderNodes = WorldGenerator.ShuffleList(nodes);

            foreach(var n in randomOrderNodes)
            {
                (bool, Vector2) result = n.HasFreeNeighbourSpace(nodes);
                if(result.Item1) return result.Item2;
            }

            Debug.LogError("Could not find a free neighbour node space. This should not be possible.");
            return Vector2.zero;
        }

        public List<LocationNode> GetNeighbours(List<LocationNode> allNeighbours)
        {
            List<LocationNode> neighbours = new();
            foreach(var d in _directions)
            {
                LocationNode neighbour = allNeighbours.Find(x => x.Position == d);
                if(neighbour != null) neighbours.Add(neighbour);
            }
            return neighbours;
        }
    }
    public (List<Location>, Grid) PlaceLocationsForest()
    {
        ForestPortalLocation forestPortalPrefab = Resources.Load<ForestPortalLocation>("Prefabs/Forest/Locations/ForestPortal");
        MeadowLocation[] meadowsPrefabs = Resources.LoadAll<MeadowLocation>("Prefabs/Forest/Locations/ForestPortal");

        List<Location> allPrefabLocations = new()
        {   
            // forestPortalPrefab,
            meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length - 1)],
            meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length - 1)],
            meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length - 1)],
            meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length - 1)],
            meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length - 1)],
        };

        List<LocationNode> nodes = new()
        {
            new(forestPortalPrefab, new(0,0)),
        };

        for(int i = 0; i < allPrefabLocations.Count - allPrefabLocations.Count ; i++)
        {
            Vector2 freeSpace = LocationNode.GetRandomFreeNeighbourSpace(nodes);
            nodes.Add(new(allPrefabLocations[0], freeSpace));
            allPrefabLocations.RemoveAt(0);
        }

        foreach(var node in nodes)
        {
            List<LocationNode> neighbours = node.GetNeighbours(nodes);
            foreach(var neighbour in neighbours)
            {
                node.Location.ConnectedLocations.Add(neighbour.Location);

                if(neighbour.Location.LocationCenterInWorld != Vector2.zero) continue;
                Vector2 diff = new(
                    (node.Location.Width / 2) + (neighbour.Location.Width / 2),
                     (node.Location.Height / 2) + (neighbour.Location.Height / 2)
                    );
                diff *= (neighbour.Position - node.Position);
                neighbour.Location.LocationCenterInWorld = node.Location.LocationCenterInWorld + diff;
            }
        }

        float maxX = 0, maxY = 0, minX = 0, minY = 0;
        foreach(LocationNode node in nodes)
        {
            if(node.Location.LocationCenterInWorld.x > maxX) maxX = node.Location.LocationCenterInWorld.x;
            if(node.Location.LocationCenterInWorld.x < minX) minX = node.Location.LocationCenterInWorld.x;
            if(node.Location.LocationCenterInWorld.y > maxY) maxY = node.Location.LocationCenterInWorld.y;
            if(node.Location.LocationCenterInWorld.y < minY) minY = node.Location.LocationCenterInWorld.y;  
        }

        int padding = 5;
        maxX += padding;
        maxY += padding;
        minX -= padding;
        minY -= padding;

        Vector2 dimensions = new((float)Math.Ceiling(maxX - minX), (float)Math.Ceiling(maxY - minY));
        List<Location> locations = nodes.Select(x => x.Location).ToList();
        Grid grid = new Grid
        {
            Cells = new CellType[(int)dimensions.x, (int)dimensions.y],
            Dimensions = dimensions,
            Offset = new(minX, minY),
            Padding = padding
        };

        return (locations, grid);
    }

    #endregion

    #region Digging Paths

    struct Path
    {
        public Vector2 Point1, Point2;
        public bool Compare(Path other)
        {
            return  (Point1 == other.Point1 && Point2 == other.Point2) ||
                    (Point1 == other.Point2 && Point2 == other.Point1);
        }
    }
    public void DigOutPaths(List<Location> locations, ref Grid grid)
    {
        List<Path> uniquePaths = new();

        foreach(var l in locations)
        {
            foreach(var neighbour in l.ConnectedLocations)
            {
                Path newPath = new Path 
                {
                    Point1 = l.LocationCenterInWorld, 
                    Point2 = neighbour.LocationCenterInWorld,
                };

                if(uniquePaths.FindAll(p => p.Compare(newPath)).Count == 0)
                {
                    uniquePaths.Add(newPath);
                }
            }
        }

        int num = 0;
        foreach(var path in uniquePaths)
        {
            // Debug.Log("=== LINE " + num + " ===");

            // Debug.Log("First point " + line.v0.Position);
            // Debug.Log("Second point " + line.v1.Position);

            num++;
            Vector2 point1 = path.Point1.x < path.Point2.x ? path.Point1 : path.Point2;
            Vector2 point2 = path.Point1.x > path.Point2.x ? path.Point1 : path.Point2;

            // Debug.Log("First point " + point1.Position);
            // Debug.Log("Second point " + point2.Position);

            float a = (point2.y - point1.y)/(point2.x - point1.x);
            // y = ax + b ----> b = y - ax
            float b = point1.y-(a * point1.x);

            int yLength = (int)Math.Abs(Math.Ceiling(a));
            yLength = yLength < 1 ? 1 : yLength; // yLength cannot be smaller than 1
            int ySymbol = a > 0 ? 1 : -1;

            //  Debug.LogFormat("yLen = {0}, a = {1}, b = {2}", yLength, a, b);
            int thickness = UnityEngine.Random.Range(2, 4);

            for(int ix = (int)Math.Floor(point1.x); ix < (int)Math.Floor(point2.x); ix++)
            {
                int x = ix;
                int indexX = x - (int)grid.Offset.x;

                for(int iy = 0; iy < yLength; iy++)
                {
                    int y = (int)Math.Floor(a*x + b)+(iy*ySymbol);
                    int indexY = y - (int)grid.Offset.y;

                    for(int tx = 0; tx < thickness; tx++)
                    {
                        for(int ty = 0; ty < thickness; ty++)
                        {
                            grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)] = CellType.Empty;
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region Mesh

    struct ChunkData
    {
        public Vector2 TopLeftCorner;
        public float Width;
        public float Height;
    }

    public void GenerateChunks(ref Grid grid)
    {
        List<ChunkData> chunkInfo = new();

        int maxChunkWidth = 10;
        int maxChunkHeight = 10;

        int widthLeft = grid.GetWidthCeil();
        int heightLeft = grid.GetHeightCeil();

        for(int ix = 0; ix < grid.GetWidthCeil() / maxChunkWidth; ix++)
        {
            for (int iy = 0; iy < grid.GetHeightCeil() / maxChunkHeight; iy++)
            {
                int chunkWidth;
                int chunkHeight;

                if(widthLeft - maxChunkWidth > 0)
                {

                }
            }
        }
    }

    #endregion

    #region Misc

    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> clonedList = new();
        list.ForEach(item => clonedList.Add(item));

        for (int i = clonedList.Count - 1; i > 0; i--)
        {
            var k = UnityEngine.Random.Range(0, i);
            var value = clonedList[k];
            clonedList[k] = clonedList[i];
            clonedList[i] = value;
        }

        return clonedList;
    }

    #endregion
}