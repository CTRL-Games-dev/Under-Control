using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum TileType
{
    Wall,
    Empty,
    Path,
}

public struct PathData
{
    public Vector2 Point1, Point2;
    public Quaternion Rotation;
    public int Thickness;
}

public struct Point // Aka vertex
{
    public Vector2 Position;
    public Location LocationOfPoint;
    public Point(Vector2 pos, Location location)
    {
        Position = pos;
        LocationOfPoint = location;
    }
    public Point(float x, float y, Location location)
    {
        Position = new Vector2(x, y);
        LocationOfPoint = location;
    }
    public static bool operator ==(Point a, Point b)
    {
        return(a.Position == b.Position);
    }
    public static bool operator !=(Point a, Point b)
    {
        return(a.Position != b.Position);
    }
}

public struct WorldData
{
    public Vector2 Offset;
    public float Scale;
    public int Width, Height;
    public TileType[,] Grid;
    public bool[,] GridUsed;
    public List<Location> Locations;
    public List<PathData> Paths;
}

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BetterGenerator : MonoBehaviour
{
    public WorldData wd;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;
    public GameObject TerrainHolder;
    public void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public T Getlocation<T>()
    where T : Location
    {
        foreach(var l in wd.Locations)
        {
            if(l.GetType() == typeof(T)) return (T)l;
        }
        return null;
    }

    public List<T> GetAllLocation<T>()
    where T : Location
    {
        List<T> locations = new();
        foreach(var l in wd.Locations)
        {
            if(l.GetType() == typeof(T)) locations.Add((T)l);
        }
        return locations;
    }

    public void GenerateMap(Dimension type)
    {
        wd.Locations = new();
        wd.Paths = new();

        TileType[,] grid;
        int gridPadding = 10;
        int gridWidth;
        int gridHeight;

        while(true)
        {
            switch(type) 
            {
                case Dimension.FOREST: {
                    PlaceLocationsForest();
                } break;
                case Dimension.FOREST_BOSS: {
                    PlaceLocationsForestBoss();
                } break;
            }

            // All locations
            int minX = 0, minY = 0, maxX = 0, maxY = 0;
            foreach(var l in wd.Locations)
            {
                if (l.TilePosX < minX) minX = l.TilePosX;
                if (l.TilePosY < minY) minY = l.TilePosY;
                if (l.TilePosX+l.TileWidth > maxX) maxX = l.TilePosX+l.TileWidth;
                if (l.TilePosY+l.TileHeight > maxY) maxY = l.TilePosY+l.TileHeight;

                Debug.Log($"Spawned location {l.Name} at {l.TilePosY}x, {l.TilePosY}");
            }

            // Grid is used to determine where to spawn trees
            gridWidth = (int)Vector2.Distance(new(minX,0), new(maxX,0)) + (gridPadding*2);
            gridHeight = (int)Vector2.Distance(new(minY,0), new(maxY,0)) + (gridPadding*2);
            // int gridHeight = (int)Vector2.Distance(new(0,minY), new(0,maxY));
            
            grid = new TileType[gridWidth + (gridPadding), gridHeight + (gridPadding)];


            // Since coordinates can be negative and we use an array
            // we need to have some kind of an offset to index this array
            Vector2 offset = new(minX-gridPadding, minY-gridPadding);

            // Set world data
            wd.Offset = offset;
            wd.Scale = 9;
            wd.Width = gridWidth;
            wd.Height = gridHeight;
            wd.Grid = grid;
            wd.GridUsed = new bool[gridWidth + (gridPadding), gridHeight + (gridPadding)];

            if(!DigOutPaths(wd.Locations, grid))
            {
                wd.Locations.Clear();
                continue;
            }

            foreach(var l in wd.Locations)
            {
                // Remove trees
                for (int ix = 0; ix < l.TileWidth; ix++)
                {
                    for (int iy = 0; iy < l.TileHeight; iy++)
                    {
                        int indexX = (int)(ix + l.TilePosX - offset.x);
                        int indexY = (int)(iy + l.TilePosY - offset.y);
                        wd.Grid[indexX, indexY] = TileType.Empty;
                        wd.GridUsed[indexX, indexY] = true;
                    }
                }
                l.GenerateLocation(TerrainHolder, wd);
            }
            break;
        }

        GameObject[] tiles = Resources.LoadAll<GameObject>("Prefabs/Forest/ForestTiles");

        #region Forest around locations
        {
            foreach(var l in wd.Locations)
            {
                int margin = 5;

                int xStart = l.TilePosX - (int)(wd.Offset.x) - margin;
                int yStart = l.TilePosY - (int)(wd.Offset.y) - margin;

                Debug.Log($"xStart = {xStart}, yStart = {yStart} ");

                xStart = xStart < 0 ? 0 : xStart;
                xStart = xStart > wd.Width ? wd.Width : xStart;

                yStart = yStart < 0 ? 0 : yStart;
                yStart = yStart > wd.Height ? wd.Height : yStart;

                int xEnd = xStart + l.TileWidth + 2 * margin;
                int yEnd = yStart + l.TileHeight + 2 * margin;

                Debug.Log($"xEnd = {xEnd}, yEnd = {yEnd} ");

                xEnd = xEnd < 0 ? 0 : xEnd;
                xEnd = xEnd > wd.Width ? wd.Width : xEnd;

                yEnd = yEnd < 0 ? 0 : yEnd;
                yEnd = yEnd > wd.Height ? wd.Height : yEnd;


                Debug.Log($"New xStart = {xStart}, yStart = {yStart} ");
                Debug.Log($"New xEnd = {xEnd}, yEnd = {yEnd} ");

                for(int x = xStart; x < xEnd; x++)
                {
                    for(int y = yStart; y < yEnd; y++)
                    {
                        if(wd.Grid[x,y] != TileType.Wall || wd.GridUsed[x,y]) { continue; }
                        float posX = x + 0.5f + UnityEngine.Random.Range(-0.1f, 0.1f);
                        float posY = y + 0.5f + UnityEngine.Random.Range(-0.1f, 0.1f);
                        Vector3 pos = new Vector3(posX, 0.0f, posY) * wd.Scale;

                        GameObject tilePrefab = tiles[UnityEngine.Random.Range(0, tiles.Length)];
                        var newTile = Instantiate(tilePrefab, pos, Quaternion.identity, TerrainHolder.transform);
                        wd.GridUsed[x,y] = true;

                        newTile.transform.eulerAngles = new Vector3(0,UnityEngine.Random.Range(-180, 180),0);
                    }
                }
            }
        }

        #endregion

        #region Forest around paths

        {
            List<Vector2> pathTiles = new();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if(wd.Grid[x,y] == TileType.Path && wd.GridUsed[x,y] == false) {
                        pathTiles.Add(new(x,y));
                        wd.GridUsed[x,y] = true;
                    }
                }
            }

            foreach(var pt in pathTiles)
            {
                int range = 3;
                for(int ix = -range; ix < range; ix++)
                {
                    for (int iy = -range; iy < range; iy++)
                    {
                        int gridX = (ix + (int)pt.x), gridY = (iy + (int)pt.y);
                        if(wd.Grid[gridX,gridY] == TileType.Wall && wd.GridUsed[gridX,gridY] == false)
                        {
                            float posX = gridX + 0.5f + UnityEngine.Random.Range(-0.1f, 0.1f);
                            float posY = gridY + 0.5f + UnityEngine.Random.Range(-0.1f, 0.1f);
                            Vector3 pos = new Vector3(posX, 0, posY) * wd.Scale;
                            GameObject tilePrefab = tiles[UnityEngine.Random.Range(0, tiles.Length)];
                            GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, TerrainHolder.transform);
                            wd.GridUsed[gridX,gridY] = true;

                            newTile.transform.eulerAngles = new Vector3(0,UnityEngine.Random.Range(-180, 180),0);
                        }
                    }
                }
            }
        }

        #endregion

        GenerateMesh(gridWidth, gridHeight);
    }

    #region Placing Locations

    private void PlaceLocationsForest()
    {
        // Place portal
        Location portal = new ForestPortal(true);
        portal.SetTileCenter(new(0,0)); // Set it to center
        wd.Locations.Add(portal);

        // Medow 
        int mCount = UnityEngine.Random.Range(3, 6);
        for(int i = 0; i < mCount; i++)
        {
            Location medow = new Medow();

            int minRange = 8;
            int maxRange = 20;
            
            int indexX = UnityEngine.Random.Range(minRange, maxRange + 1);
            int indexY = UnityEngine.Random.Range(minRange, maxRange + 1);

            indexX *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            indexY *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

            bool flag = false;
            for(int t = 0; t < 20; t++)
            {
                medow.SetTileCenter(new(indexX, indexY));
                if(medow.CheckLocation(wd.Locations)) {
                    flag = true;
                    Debug.Log("Found location");
                    break;
                };
            }

            if(flag) 
            {
                wd.Locations.Add(medow);
            }
            else Debug.Log("Didnt found location!");
        }
    }

    private void PlaceLocationsForestBoss()
    {
        // Place portal
        Location portal = new ForestPortal(false);
        portal.SetTileCenter(new(0,0)); // Set it to center
        wd.Locations.Add(portal);


        Location arena = new ForestBossArena();
        arena.SetTileCenter(new(3,3)); // Set it to center
        wd.Locations.Add(arena);
    }

    #endregion

    #region Mesh

    private void GenerateMesh(int gridWidth, int gridHeight)
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

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++) {
                int index = x * gridHeight + y;

                Vector3 p0 = new Vector3(x,     th[x,y],  y) * wd.Scale;
                Vector3 p1 = new Vector3(x,     th[x,y+1],  (y+1)) * wd.Scale;
                Vector3 p2 = new Vector3((x+1), th[x+1,y+1],  (y+1)) * wd.Scale;
                Vector3 p3 = new Vector3((x+1), th[x+1,y], y) * wd.Scale;

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

    #endregion

    private bool DigOutPaths(List<Location> locations, TileType[,] grid) 
    {
        List<Edge> uniqueEdges = new();

        if(locations.Count > 2)
        {
            // Create "sigma triangle"
            Triangle st = Triangle.GetSuperTriangle(locations);

            List<Triangle> triangles = new();
            triangles.Add(st);

            // Triangulate each vertex (magic)
            foreach (var location in locations)
            {
                Point vertex = new(location.GetTileCenterWithoutOffset(), location);
                triangles = AddVertex(vertex, triangles);
            }

            // Remove triangles that share edges with sigma triangle (they are not so sigma)
            triangles = triangles.Where(t => 
                !(t.vA == st.vA || t.vA == st.vB || t.vA == st.vC ||
                t.vB == st.vA || t.vB == st.vB || t.vB == st.vC ||
                t.vC == st.vA || t.vC == st.vB || t.vC == st.vC)
            ).ToList();

            uniqueEdges = RemoveDuplicateEdges(triangles);
        } 
        else
        {
            Vector2 pos1 = new(locations[0].TilePosX, locations[0].TilePosY);
            Vector2 pos2 = new(locations[1].TilePosX, locations[1].TilePosY);
            uniqueEdges .Add(new Edge(new Point(pos1, locations[0]), new Point(pos2, locations[1])));
        }
        

        #region MST

        if(uniqueEdges.Count == 0) {
            Debug.Log("Cannot generate world!");
            return false;
        }
        Edge firstEdge = uniqueEdges.Aggregate(uniqueEdges[0], (smallest, next) => {
            return smallest.Length > next.Length ? next : smallest;
        });

        firstEdge.MarkAsUsed();

        // Do this as long as there are unconnected edges
        // If edge is not connected from both sides, this means that at least one of it's points is not connected
        List<Edge> usedEdges = uniqueEdges.Where(e => e.Used).ToList(); // Used in some later computations
        while(uniqueEdges.FindAll(e => !e.IsConnected(usedEdges)).Count > 0 && uniqueEdges.Count > 2)
        {
            usedEdges = uniqueEdges.Where(e => e.Used).ToList(); // Used in some later computations

            // Get all edges that are connected to already used edges 
            // and set sort them in order (so if one is not okay, we can skip it and go to the next one)
            List<Edge> connectedEdges = uniqueEdges
                .FindAll(e => !e.Used && e.IsOnlyPartiallyConnected(usedEdges))
                .OrderBy(e => e.Length).ToList();

            // Debug.Log("=== List of connected edges after sorting ===");
            foreach(var ce in connectedEdges)
            {
                // Debug.LogFormat("Connected edge: {0}-{1}, {2}", ce.v0, ce.v1, ce.Length);
            }

            foreach(var shortest in connectedEdges)
            {   
                // Debug.LogFormat("Next shortest edge: {0}-{1}, {2}", shortest.v0, shortest.v1, shortest.Length);
                if(!shortest.IsFullyConnected(usedEdges)) {
                    shortest.MarkAsUsed();
                    break;
                }
                // Debug.Log("Edge was rejected!");
            }
        }

        // Debug.Log("Fully connected edges: " + uniqueEdges.Where(e => e.Used).Count());
        // foreach(var e in uniqueEdges.Where(e => e.Used))
        // {
        //     Debug.LogFormat("vector({0},{1})", e.v0, e.v1);
        // }
        #endregion

        // Small chance for other edges also to be used
        foreach(var e in uniqueEdges.Where(e => !e.Used))
        {
            if(UnityEngine.Random.Range(0, 3) == 1) {
                e.MarkAsUsed();
                // Debug.LogFormat("Random edge added: ({0})-({1})", e.v0, e.v1);
            }
        }

        #region Rasterization

        int num = 0;
        foreach(var line in uniqueEdges.Where(e => e.Used))
        {
            Debug.Log("=== LINE " + num + " ===");

            Debug.Log("First point " + line.v0.Position);
            Debug.Log("Second point " + line.v1.Position);

            num++;
            Point point1 = line.v0.Position.x < line.v1.Position.x ? line.v0 : line.v1;
            Point point2 = line.v0.Position.x > line.v1.Position.x ? line.v0 : line.v1;

            Debug.Log("First point " + point1.Position);
            Debug.Log("Second point " + point2.Position);

            float a = (point2.Position.y - point1.Position.y)/(point2.Position.x - point1.Position.x);
            // y = ax + b ----> b = y - ax
            float b = point1.Position.y-(a * point1.Position.x);

            int yLength = (int)Math.Abs(Math.Ceiling(a));
            yLength = yLength < 1 ? 1 : yLength; // yLength cannot be smaller than 1
            int ySymbol = a > 0 ? 1 : -1;

            //  Debug.LogFormat("yLen = {0}, a = {1}, b = {2}", yLength, a, b);
            int thickness = UnityEngine.Random.Range(2, 4);

            for(int ix = (int)Math.Floor(point1.Position.x); ix < (int)Math.Floor(point2.Position.x); ix++)
            {
                int x = ix;
                int indexX = x - (int)wd.Offset.x;

                for(int iy = 0; iy < yLength; iy++)
                {
                    int y = (int)Math.Floor(a*x + b)+(iy*ySymbol);
                    int indexY = y - (int)wd.Offset.y;

                    for(int tx = 0; tx < thickness; tx++)
                    {
                        for(int ty = 0; ty < thickness; ty++)
                        {
                            wd.Grid[indexX+tx-(thickness/2),indexY+ty-(thickness/2)] = TileType.Path;
                        }
                    }
                }
            }


            // Vector2[,] edgesA = point1.LocationOfPoint.GetEdges();
            // Vector2 intersectionA = new Vector2(555555,555555); // Funny big vector if an error happens;
            // bool foundA = false;
            // Debug.Log($"Line pA = (({point1.Position}),({point2.Position}))");
            // for(int i = 0; i < 4; i++)
            // {
            //     Edge locationEdge = new(new(edgesA[i,0],point1.LocationOfPoint), new(edgesA[i,1],point1.LocationOfPoint));
            //     Edge pathLine = new(point1, point2);
            //     (bool ifFound, Vector2 intersection) = FindIntersection(locationEdge, pathLine);
            //     Debug.Log($"Loc1, (({locationEdge.v0.Position}),({locationEdge.v1.Position}))");
            //     if(ifFound)
            //     {
            //         intersectionA = intersection;
            //         foundA = true;
            //         break;
            //     }
            // }

            // Vector2[,] edgesB = point2.LocationOfPoint.GetEdges();
            // Vector2 intersectionB = new Vector2(555555,555555); // Funny big vector if an error happens
            // bool foundB = false;
            // for(int i = 0; i < 4; i++)
            // {
            //     Edge locationEdge = new(new(edgesB[i,0],point2.LocationOfPoint), new(edgesB[i,1],point2.LocationOfPoint));
            //     Debug.Log($"Loc2, (({locationEdge.v0.Position}),({locationEdge.v1.Position}))");
            //     Edge pathLine = new(point1, point2);
            
            //     (bool ifFound, Vector2 intersection) = FindIntersection(locationEdge, pathLine);
            //     if(ifFound)
            //     {
            //         intersectionB = intersection;
            //         foundB = true;
            //         break;
            //     }
            // }

            // if(!(foundA && foundB))
            // {
            //     return false
            //     // throw new Exception("Generator fell from it's bicycle, oopsie");
            // }

            // PathData path;
            // path.Point1 = intersectionA;
            // path.Point2 = intersectionB;
            // path.Thickness = thickness;
            // float len = Vector2.Distance(path.Point1, path.Point2);

            // Vector2 dir = point2.Position - point1.Position; //a vector pointing from pointA to pointB
            // path.Rotation = Quaternion.LookRotation(new(dir.x, 0, dir.y), Vector3.up); //calc a rotation that

            // wd.Paths.Add(path);
        }

        return true;
        #endregion
    }

    #region MST helpers

    private List<Edge> RemoveDuplicateEdges(List<Edge> edges)
    {
        List<Edge> uniqueEdges = new();
        foreach(var e in edges)
        {
            if(uniqueEdges.Find(unique => unique.Equals(e)) == null) uniqueEdges.Add(e);
        }
        return uniqueEdges;
    }
    private List<Edge> RemoveDuplicateEdges(List<Triangle> triangles)
    {
        List<Edge> edges = new();
        triangles.ForEach(t => edges.AddRange(t.GetEdges()));
        return RemoveDuplicateEdges(edges);
    }

    // https://stackoverflow.com/questions/13937782/calculating-the-point-of-intersection-of-two-lines
    private (bool, Vector2) FindIntersection(Edge lineA, Edge lineB, float tolerance = 0.001f)
    {

        float x1 = lineA.v0.Position.x;
        float x2 = lineA.v1.Position.x;
        float x3 = lineB.v0.Position.x;
        float x4 = lineB.v1.Position.x;
        float y1 = lineA.v0.Position.y;
        float y2 = lineA.v1.Position.y;
        float y3 = lineB.v0.Position.y;
        float y4 = lineB.v1.Position.y;

        // Check if none of the lines are of length 0
        if ((x1 == x2 && y1 == y2) || (x3 == x4 && y3 == y4)) {
            return (false, Vector2.zero);
        }

        float denominator = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

        // Lines are parallel
        if (denominator == 0) {
            return (false, Vector2.zero);
        }

        float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
        float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;

        // is the intersection along the segments
        if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
            return (false, Vector2.zero);;
        }

        // Return a object with the x and y coordinates of the intersection
        float x = x1 + ua * (x2 - x1);
        float y = y1 + ua * (y2 - y1);

        return (true, new Vector2(x,y));
    }

    #endregion

    #region Triangulation helper functions
    private List<Triangle> AddVertex(Point vertex, List<Triangle> triangles)
    {
        List<Edge> edges = new();

        // Remove triangles with circumcircles containing the vertex
        triangles = triangles.Where(t => {
            // Debug.Log("triangle center: " + t.vCenter);
            if (t.InCircumcircle(vertex.Position)) {
                // Debug.Log("Bad triangle!");
                edges.Add(new Edge(t.vA, t.vB));
                edges.Add(new Edge(t.vB, t.vC));
                edges.Add(new Edge(t.vC, t.vA));
                return false;
            }
            return true;
        }).ToList();
        // Debug.Log("Number of triangles inside iteration: " + triangles.Count);

        // Get unique edges
        List<Edge> uniqueEdges = GetUniqueEdges(edges);

        foreach(var e in edges)
        {
            triangles.Add(new Triangle(e.v0, e.v1, vertex));
        }

        return triangles;
    }
    private List<Edge> GetUniqueEdges(List<Edge> edges)
    {
        List<Edge> uniqueEdges = new();
        for (var i = 0; i < edges.Count; ++i) {
            var isUnique = true;

            // See if edge is unique
            for (var j = 0; j < edges.Count; ++j) {
                if (i != j && edges[i].Equals(edges[j])) {
                    isUnique = false;
                    break;
                }
            }

            // Edge is unique, add to unique edges array
            if(isUnique) uniqueEdges.Add(edges[i]);
        }
        return uniqueEdges;
    }
    private List<Edge> GetUniqueEdges(List<Triangle> triangles)
    {
        List<Edge> edges = new();
        triangles.ForEach(t => {
            edges.Add(new Edge(t.vA, t.vB));
            edges.Add(new Edge(t.vB, t.vC));
            edges.Add(new Edge(t.vC, t.vA));
        });
        return GetUniqueEdges(edges);
    }

    #endregion

    #region Triangles

    // === I LOVE TRIANGLES :3 ===
    // https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/#the-super-triangle
    
    private class Edge
    {
        // Variables and methods used for triangulation
        public Point v0, v1;
        public Edge(Point v0, Point v1)
        {
            this.v0 = v0; this.v1 = v1;
            this.Length = Vector2.Distance(v0.Position, v1.Position);
        }
        public bool Equals(Edge other)
        {
            return (v0 == other.v0 && v1 == other.v1) || 
                (v0 == other.v1 && v1 == other.v0);
        }

        // public void MarkAsRedudnant()
        // {
        //     NotNeeded = true;
        // }

        // Variables and methods used for MST
        public float Length;
        public bool Used { get; private set; } = false; // Used to say that this edge is being used

        // Used to mark this path as no longer useful
        // public bool NotNeeded { get; private set; } = false ;

        // This function only checks if other edge is connected with this edge
        public bool ConnectedWith(Edge other)
        {
            return v0 == other.v0 || v1 == other.v1 || 
                v0 == other.v1 || v1 == other.v0;
        }

        // This function only checks edge is connected at least from one side
        public bool IsConnected(List<Edge> otherEdges)
        {
            bool vA = false, vB = false;
            foreach(var other in otherEdges)
            {
                if(this == other ) continue;
                if(v0 == other.v0 || v0 == other.v1) vA = true;
                if(v1 == other.v0 || v1 == other.v1) vB = true;
            }
            // Debug.Log("vA = " + vA + ", vB = " + vB);
            return vA || vB;
        }
        // This function only checks edge is connected only from one side
        public bool IsOnlyPartiallyConnected(List<Edge> otherEdges)
        {
            bool vA = false, vB = false;
            foreach(var other in otherEdges)
            {
                if(this == other ) continue;
                if(v0 == other.v0 || v0 == other.v1) vA = true;
                if(v1 == other.v0 || v1 == other.v1) vB = true;
            }
            // Debug.Log("vA = " + vA + ", vB = " + vB);
            return vA != vB;
        }
        // This function only checks edge is connected from both sides
        public bool IsFullyConnected(List<Edge> otherEdges)
        {
            bool vA = false, vB = false;
            foreach(var other in otherEdges)
            {
                if(this == other) continue;
                if(v0 == other.v0 || v0 == other.v1) vA = true;
                if(v1 == other.v0 || v1 == other.v1) vB = true;
            }
            return vA && vB;
        }
        // This function checks if other edge and marks it, if it is
        public void MarkAsUsed()
        {
            Used = true;
            // NotNeeded = true;
        }
    }

    private class Triangle
    {
        public Point vA, vB, vC;
        public Vector2 vCenter;
        public Triangle(Point vA, Point vB, Point vC)
        {
            this.vA = vA; this.vB = vB; this.vC = vC;
            this.vCenter = GetCircumcenter(vA.Position, vB.Position, vC.Position);
        }

        public static Triangle GetSuperTriangle(List<Location> locations)
        {
            float minX, minY, maxX, maxY;
            minX = minY = 100000000000; // some big number
            maxX = maxY = -100000000000; // some big number
            foreach(var l in locations)
            {
                Vector2 vertex = l.GetTileCenterWithoutOffset();
                minX = Math.Min(minX, vertex.x);
                minY = Math.Min(minX, vertex.y);
                maxX = Math.Max(maxX, vertex.x);
                maxY = Math.Max(maxX, vertex.y);
            }
            var dx = (maxX - minX) * 10;
            var dy = (maxY - minY) * 10;

            var v0 = new Point(minX - dx, minY - dy * 3, new DummyLocation(new(25555,2555),new(25555,2555)));
            var v1 = new Point(minX - dx, maxY + dy, new DummyLocation(new(25555,2555),new(25555,2555)));
            var v2 = new Point(maxX + dx * 3, maxY + dy, new DummyLocation(new(25555,2555),new(25555,2555)));

            return new Triangle(v0, v1, v2);
        }

        public List<Edge> GetEdges()
        {
            return new()
            {
                new Edge(vA, vB),
                new Edge(vB, vC),
                new Edge(vC, vA)
            };
        }
        public bool InCircumcircle(Vector2 v)
        {
            var dx = vCenter.x - v.x;
            var dy = vCenter.y - v.y;
            var radius = Vector2.Distance(vCenter, vA.Position);
            return Math.Sqrt(dx * dx + dy * dy) <= radius;
        }

        // What the fuck is going on?
        // https://www.reddit.com/r/Unity3D/comments/wppjjd/how_to_calculate_the_circumcenter_of_a_triangle/
        Vector2 GetCircumcenter(Vector2 vA, Vector2 vB, Vector2 vC)
        {
            LinearEquation lineAB = new LinearEquation(vA, vB);
            LinearEquation lineBC = new LinearEquation(vB, vC);

            Vector2 midPointAB = Vector2.Lerp(vA, vB, 0.5f);
            Vector2 midPointBC = Vector2.Lerp(vB, vC, 0.5f);

            LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
            LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);

            return GetCrossingPoint(perpendicularAB, perpendicularBC);
        }
        private class LinearEquation 
        {
            public float _A, _B, _C;
            public LinearEquation() {}
            public LinearEquation(Vector2 vA, Vector2 vB)
            {
                float deltaX = vB.x - vA.x;
                float deltaY = vB.y - vA.y;
                _A = deltaY;
                _B = -deltaX;
                _C = _A * vA.x + _B * vA.y;
            }

            public LinearEquation PerpendicularLineAt(Vector3 point)
            {
                LinearEquation newLine = new LinearEquation();
                newLine._A = -_B;
                newLine._B = _A;
                newLine._C = newLine._A * point.x + newLine._B * point.y;
                return newLine;
            }
        }
        Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
        {
            float A1 = line1._A;
            float A2 = line2._A;
            float B1 = line1._B;
            float B2 = line2._B;
            float C1 = line1._C;
            float C2 = line2._C;


            // Cramer's rule
            float Determinant = A1 * B2 - A2 * B1;
            float DeterminantX = C1 * B2 - C2 * B1;
            float DeterminantY = A1 * C2 - A2 * C1;

            float x = DeterminantX / Determinant;
            float y = DeterminantY / Determinant;
            return new Vector2(x, y);
        }
    }

    #endregion
}