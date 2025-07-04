
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public struct WorldCell
{
    public bool WasPlaced;
    public CellType Type;
}
public enum CellType
{
    Empty,
    Forest,
    DeadZone,
}

public struct Grid
{
    public WorldCell[,] Cells;
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
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private GameObject _terrainHolder;
    [SerializeField] private GameObject _terrainChunkHolder;
    [HideInInspector] public List<Location> PlacedLocations = new();

    public T GetLocation<T>()
    where T : Location
    {
        foreach(var l in PlacedLocations)
        {
            if(l.GetType() == typeof(T)) return (T)l;
        }
        return null;
    }

    public List<T> GetAllLocations<T>()
    where T : Location
    {
        List<T> locations = new();
        foreach(var l in PlacedLocations)
        {
            if(l.GetType() == typeof(T)) locations.Add((T)l);
        }
        return locations;
    }
    public void GenerateMap(Dimension type)
    {

        Grid grid;
        List<Location> locations;

        switch(type)
        {
            case Dimension.FOREST: {
                (locations, grid) = PlaceLocationsForest();
            } break;
            default: {
                Debug.LogError($"This type of dimension is not implemented ({type}). Spawning regular forest");
                (locations, grid) = PlaceLocationsForest();
            } break;
        }

        Debug.Log("Locations have been placed");

        
        Debug.Log("Placing trees");
        PlaceForestAroundLocations(locations, ref grid);
        DigOutPaths(locations, ref grid);
        Debug.Log("Trees are in place");

        Debug.Log("Spawning locations");
        SetProperLocationCoordinates(locations);
        PlaceForestTiles(ref grid);
        Debug.Log("Spawned locations");

        // GenerateChunks(ref grid);
        // Debug.Log("Generated mesh chunks");
    }


    #region Dimension types

    class LocationNode
    {
        public Location Location;
        public Vector2 Position;
        private List<Vector2> _directions;
        public bool AlreadyPlaced = false;
        public LocationNode(Location l, Vector2 pos)
        {
            Location = l;
            Position = pos;

            _directions = new()
            {
                new(Position.x, Position.y + 1), // Up
                new(Position.x + 1, Position.y), // Right
                new(Position.x, Position.y - 1), // Down
                new(Position.x - 1, Position.y), // Left
            };
        }

        public (bool, Vector2) HasFreeNeighbourSpace(List<LocationNode> allNodes)
        {

            var shuffledDirections = FluffyUtils.ShuffleList<Vector2>(_directions);

            foreach(var d in shuffledDirections) 
            {
                if(allNodes.Find(x => x.Position == d) == null) return (true, d);
            }

            return (false, Vector2.zero);
        }
        public static Vector2 GetRandomFreeNeighbourSpace(List<LocationNode> nodes)
        {
            List<LocationNode> randomOrderNodes = FluffyUtils.ShuffleList(nodes);

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
        Location forestSpawnPrefab = Resources.Load<Location>("Prefabs/Forest/Locations/ForestSpawn");
        Location[] meadowsPrefabs = Resources.LoadAll<Location>("Prefabs/Forest/Locations/Meadows");
        Location[] rockyFieldsPrefabs = Resources.LoadAll<Location>("Prefabs/Forest/Locations/RockyFields");
        Location wellPrefab = Resources.Load<Location>("Prefabs/Forest/Locations/Well");
        Location sellerPrefab = Resources.Load<Location>("Prefabs/Forest/Locations/Seller");
        Location pondPrefab = Resources.Load<Location>("Prefabs/Forest/Locations/FishingPond");

        Debug.Log("Loaded location prefabs");

        List<Location> allLocations = new();

        int numberOfMeadows = UnityEngine.Random.Range(4, 7);
        int numberOfRockyFields = UnityEngine.Random.Range(1, 3);
        int numberofPortals = UnityEngine.Random.Range(2, 4);

        for(int i = 0; i < numberOfMeadows; i++)
        {
            allLocations.Add(Instantiate(meadowsPrefabs[UnityEngine.Random.Range(0, meadowsPrefabs.Length)], Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        }

        for(int i = 0; i < numberOfRockyFields; i++)
        {
            allLocations.Add(Instantiate(rockyFieldsPrefabs[UnityEngine.Random.Range(0, rockyFieldsPrefabs.Length)], Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        }

        for(int i = 0; i < numberofPortals; i++)
        {
            allLocations.Add(Instantiate(forestPortalPrefab, Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        }

        allLocations.Add(Instantiate(sellerPrefab, Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        allLocations.Add(Instantiate(wellPrefab, Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        allLocations.Add(Instantiate(pondPrefab, Vector3.zero, Quaternion.identity, _terrainHolder.transform));

        List<LocationNode> nodes = new()
        {
            new(Instantiate(forestSpawnPrefab, Vector3.zero, Quaternion.identity, _terrainHolder.transform), new(0,0)),
        };

        // Location[] testLocationsPrefabs = Resources.LoadAll<Location>("Prefabs/Forest/Locations/TestLocations");
        // for(int i = 0; i < 10; i++)
        // {
        //     allLocations.Add(Instantiate(testLocationsPrefabs[UnityEngine.Random.Range(0, testLocationsPrefabs.Length)], Vector3.zero, Quaternion.identity, _terrainHolder.transform));
        // }

        // Get place in a grid of locations
        while(allLocations.Count != 0)
        {
            Vector2 freeSpace = LocationNode.GetRandomFreeNeighbourSpace(nodes);
            nodes.Add(new(allLocations[0], freeSpace));
            allLocations.RemoveAt(0);
        }

        // Since locations have different sizes,
        // they need to be taken into consideration when placing things from the grid into the real world

        List<LocationNode> nodesToCheck = new();

        LocationNode firstNode = nodes[0]; // First node
        firstNode.Location.LocationCenterInWorld = firstNode.Position;
        firstNode.AlreadyPlaced = true;

        nodesToCheck.Add(firstNode);

        while(nodesToCheck.Count != 0)
        {
            LocationNode currentNode = nodesToCheck[0];
            nodesToCheck.RemoveAt(0);

            List<LocationNode> neighbours = currentNode.GetNeighbours(nodes);
            foreach(var neighbour in neighbours)
            {
                currentNode.Location.ConnectedLocations.Add(neighbour.Location);

                if(neighbour.AlreadyPlaced) continue;

                // Additional padding for distance between locations
                Vector2 diff = new Vector2(60,60);
                
                // This is very important.
                // Diff by default applies to all axis and can only have positive value.
                // This will make difference only contain one, relevant axis. It will also invert the value, if neighbour is under the current node 
                diff *= neighbour.Position - currentNode.Position;

                neighbour.Location.LocationCenterInWorld = currentNode.Location.LocationCenterInWorld + diff;

                neighbour.AlreadyPlaced = true;
                nodesToCheck.Add(neighbour);
            }
        }

        float maxX = 0, maxY = 0, minX = 0, minY = 0;
        foreach(LocationNode node in nodes)
        {
            if(node.Location.GetTopRightCorner().x > maxX) maxX = node.Location.GetTopRightCorner().x;
            if(node.Location.GetTopLeftCorner().x < minX) minX = node.Location.GetTopLeftCorner().x;
            if(node.Location.GetBottomLeftCorner().y > maxY) maxY = node.Location.GetBottomLeftCorner().y;
            if(node.Location.GetTopLeftCorner().y < minY) minY = node.Location.GetTopLeftCorner().y;  
        }

        int padding = 60;
        maxX += padding;
        maxY += padding;
        minX -= padding;
        minY -= padding;

        Vector2 dimensions = new((float)Math.Ceiling(maxX - minX), (float)Math.Ceiling(maxY - minY));
        List<Location> locations = nodes.Select(x => x.Location).ToList();
        Grid grid = new Grid
        {
            Cells = new WorldCell[(int)dimensions.x, (int)dimensions.y],
            Dimensions = dimensions,
            Offset = new(minX, minY),
            Padding = padding,
        };

        return (locations, grid);
    }

    #endregion

    #region Digging Paths and Placing Forest

    struct Path
    {
        public Vector2 Point1, Point2;
        public bool Compare(Path other)
        {
            return  (Point1 == other.Point1 && Point2 == other.Point2) ||
                    (Point1 == other.Point2 && Point2 == other.Point1);
        }
    }

    public void PlaceForestAroundLocations(List<Location> locations, ref Grid grid)
    {
        foreach(Location l in locations)
        {
            // Place forest
            int margin = grid.Padding; // Margin cannot be bigger that padding (otherwise may index grid out of bounds)
            for(int ix = -margin; ix < (int)l.Width + margin - 1; ix++)
            {

                for (int iy = -margin; iy < (int)l.Height + margin - 1 ; iy++)
                {
                    Vector2 pos = l.GetTopLeftCorner();

                    int x = (int)(pos.x + ix - grid.Offset.x);
                    int y = (int)(pos.y + iy - grid.Offset.y);

                    if(grid.Cells[x,y].WasPlaced) continue;

                    // Debug.Log($"Pos: {pos}, Offset: {grid.Offset}");
                    // Debug.Log($"ix: {ix}, iy: {iy}");
                    // Debug.Log($"Index: {new Vector2(x, y)}");

                    grid.Cells[x, y] = new WorldCell
                    {
                        Type = CellType.DeadZone,
                        WasPlaced = true
                    };
                }
            }

            int locationOffset = 12;

            // Dig out the actual location
            for(int ix = -locationOffset; ix < (int)l.Width + locationOffset - 1; ix++)
            {
                for (int iy = -locationOffset; iy < (int)l.Height + locationOffset - 1; iy++)
                {
                    Vector2 pos = l.GetTopLeftCorner();
                    int x = (int)(pos.x + ix - grid.Offset.x);
                    int y = (int)(pos.y + iy - grid.Offset.y);

                    // Debug.Log($"Index: {new Vector2(x, y)}");

                    if(ix >= 0 && ix < l.Width && iy >= 0 && iy < l.Height) {
                        grid.Cells[x,y] = new WorldCell { Type = CellType.Empty, WasPlaced = true };   
                    } else {
                        grid.Cells[x,y] = new WorldCell { Type = CellType.Forest, WasPlaced = true };
                    }
                }
            }
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

        // int num = 0;
        foreach(var path in uniquePaths)
        {
            // Debug.Log("=== LINE " + num + " ===");

            // Debug.Log("First point " + path.Point1);
            // Debug.Log("Second point " + path.Point2);

            // num++;
            Vector2 point1 = path.Point1.x < path.Point2.x ? path.Point1 : path.Point2;
            Vector2 point2 = path.Point1.x >= path.Point2.x ? path.Point1 : path.Point2;

            // Debug.Log("First point " + point1);
            // Debug.Log("Second point " + point2);

            int thickness = UnityEngine.Random.Range(10, 15);


            int locationOffset = 12;
            if(point2.x != point1.x)
            {
                float a = (point2.y - point1.y)/(point2.x - point1.x);
                // y = ax + b ----> b = y - ax
                float b = point1.y-(a * point1.x);

                int yLength = (int)Math.Abs(Math.Ceiling(a));
                yLength = yLength < 1 ? 1 : yLength; // yLength cannot be smaller than 1
                int ySymbol = a > 0 ? 1 : -1;

                // Debug.LogFormat("yLen = {0}, a = {1}, b = {2}", yLength, a, b);


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
                                grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)] = new WorldCell { Type = CellType.Empty };
                            }
                        }

                        for(int tx = -locationOffset; tx < thickness + locationOffset; tx++)
                        {
                            for(int ty = -locationOffset; ty < thickness + locationOffset; ty++)
                            {
                                WorldCell cell = grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)];
                                if(cell.Type == CellType.DeadZone) {
                                    grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)] = new WorldCell { Type = CellType.Forest };
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                // Handle vertical line separately
                int startY = Mathf.FloorToInt(Mathf.Min(point1.y, point2.y));
                int endY = Mathf.FloorToInt(Mathf.Max(point1.y, point2.y));
                int x = Mathf.FloorToInt(point1.x);  // or point2.x since they are the same

                // Loop over the Y values of the vertical line
                for (int iy = startY; iy <= endY; iy++)
                {
                    int indexX = x - (int)grid.Offset.x;
                    int indexY = iy - (int)grid.Offset.y;

                    for (int tx = 0; tx < thickness; tx++)
                    {
                        for (int ty = 0; ty < thickness; ty++)
                        {
                            grid.Cells[indexX + tx - (thickness / 2), indexY + ty - (thickness / 2)] = new WorldCell { Type = CellType.Empty };
                        }
                    }

                    for(int tx = -locationOffset; tx < thickness + locationOffset; tx++)
                    {
                        for(int ty = -locationOffset; ty < thickness + locationOffset; ty++)
                        {
                            WorldCell cell = grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)];
                            if(cell.Type == CellType.DeadZone)
                                grid.Cells[indexX+tx-(thickness/2),indexY+ty-(thickness/2)] = new WorldCell { Type = CellType.Forest };
                        }
                    }
                }
            }
            
        }
    }

    #endregion

    #region Location Placing

    public void SetProperLocationCoordinates(List<Location> locations)
    {
        foreach(Location l in locations)
        {   
            Debug.Log($"Placing location {l.Name} at {l.LocationCenterInWorld}");
            l.transform.position = new(l.LocationCenterInWorld.x, 0, l.LocationCenterInWorld.y);
            PlacedLocations.Add(l);
        }
    }

    public void PlaceForestTiles(ref Grid grid)
    {
        Location[] forestTilePrefabs = Resources.LoadAll<Location>("Prefabs/Forest/Locations/ForestTiles");

        int spaceBetweenTrees = 4;

        for (int ix = 0; ix < grid.Cells.GetLength(0) / spaceBetweenTrees; ix++)
        {
            for (int iy = 0; iy < grid.Cells.GetLength(1) / spaceBetweenTrees; iy++)
            {
                int gridPositionX = ix * spaceBetweenTrees;
                int gridPositionY = iy * spaceBetweenTrees;

                switch(grid.Cells[gridPositionX, gridPositionY].Type) {
                    case CellType.Empty: continue;
                    case CellType.Forest: {
                        float worldPositionX = gridPositionX + grid.Offset.x;
                        float worldPositionY = gridPositionY + grid.Offset.y;

                        Location forestTilePrefab = forestTilePrefabs[UnityEngine.Random.Range(0, forestTilePrefabs.Length)];
                        if(forestTilePrefab is ForestBorder borderPrefab) {
                            Instantiate(borderPrefab, new Vector3(worldPositionX + 0.5f , 0, worldPositionY + 0.5f), Quaternion.identity, _terrainHolder.transform)
                                .SetDeadZone(false);
                        }
                    } break;
                    case CellType.DeadZone: {
                        float worldPositionX = gridPositionX + grid.Offset.x;
                        float worldPositionY = gridPositionY + grid.Offset.y;

                        Location forestTilePrefab = forestTilePrefabs[UnityEngine.Random.Range(0, forestTilePrefabs.Length)];
                        if(forestTilePrefab is ForestBorder borderPrefab) {
                            Instantiate(borderPrefab, new Vector3(worldPositionX + 0.5f , 0, worldPositionY + 0.5f), Quaternion.identity, _terrainHolder.transform)
                                .SetDeadZone(true);
                        }
                    } break;
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

    public void GenerateChunks(ref Grid grid) {
        List<ChunkData> chunkInfo = new();

        int maxChunkWidth = 100;
        int maxChunkHeight = 100;

        int gridWidth = grid.GetWidthCeil();
        int gridHeight = grid.GetHeightCeil();

        int numChunksX = (int)Math.Ceiling((float)gridWidth / maxChunkWidth);
        int numChunksY = (int)Math.Ceiling((float)gridHeight / maxChunkHeight);

        for (int ix = 0; ix < numChunksX; ix++)
        {
            for (int iy = 0; iy < numChunksY; iy++)
            {
                // Calculate remaining width/height for edge chunks
                int chunkWidth = Math.Min(maxChunkWidth, gridWidth - ix * maxChunkWidth);
                int chunkHeight = Math.Min(maxChunkHeight, gridHeight - iy * maxChunkHeight);

                float x = ix * maxChunkWidth + grid.Offset.x;
                float y = iy * maxChunkHeight + grid.Offset.y;

                chunkInfo.Add(new ChunkData
                {
                    TopLeftCorner = new(x, y),
                    Width = chunkWidth,
                    Height = chunkHeight,
                });
            }
        }

        foreach (var ci in chunkInfo)
        {
            Chunk chunk = Instantiate(_chunkPrefab, Vector3.zero, Quaternion.identity, _terrainChunkHolder.transform);
            chunk.GenerateChunkMesh(ci.TopLeftCorner, (int)ci.Width, (int)ci.Height);
        }
    }


    #endregion
}