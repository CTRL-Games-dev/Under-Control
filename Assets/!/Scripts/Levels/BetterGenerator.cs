using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public struct Tile
{
    public bool IsWall;
    public int X, Y;
}

public struct WorldData
{
    public Vector2 Offset;
    public float Scale;
    public int Width, Height;

    // public WorldData(Vector2 os, float sc)
    // {
    //     Offset = os;
    //     Scale = sc;
    // }
}

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BetterGenerator : MonoBehaviour
{
    List<Location> allLocations = new();
    public WorldData wd;
    private MeshFilter _mf;
    private MeshCollider _mc;
    private MeshRenderer _mr;
    public GameObject TerrainHolder;
    public void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mc = GetComponent<MeshCollider>();
        _mr = GetComponent<MeshRenderer>();
    }
    public enum LevelType
    {
        Forest
    }
    public void GenerateMap(LevelType type)
    {
        switch(type)
        {
            case LevelType.Forest: {
                GenerateForest();
            } break;
        }
    }

    public T Getlocation<T>()
    where T : Location
    {
        foreach(var l in allLocations)
        {
            if(l.GetType() == typeof(T)) return (T)l;
        }
        return null;
    }

    public List<T> GetAllLocation<T>()
    where T : Location
    {
        List<T> locations = new();
        foreach(var l in allLocations)
        {
            if(l.GetType() == typeof(T)) locations.Add((T)l);
        }
        return locations;
    }

    private void GenerateForest()
    {
        PlaceLocations(allLocations);

        // All locations
        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        foreach(var l in allLocations)
        {
            if (l.X < minX) minX = l.X;
            if (l.Y < minY) minY = l.Y;
            if (l.X+l.TileWidth > maxX) maxX = l.X+l.TileWidth;
            if (l.Y+l.TileHeight > maxY) maxY = l.Y+l.TileHeight;

            Debug.Log("Spawned location at x->" + l.X + " y->" + l.Y);
        }

        // Grid is used to determine where to spawn trees
        int gridPadding = 5;
        int gridWidth = (int)Vector2.Distance(new(minX,0), new(maxX,0)) + (gridPadding*2);
        int gridHeight = (int)Vector2.Distance(new(minY,0), new(maxY,0)) + (gridPadding*2);;
        // int gridHeight = (int)Vector2.Distance(new(0,minY), new(0,maxY));
        
        bool[,] grid = new bool[gridWidth + (gridPadding*2), gridHeight + (gridPadding*2)];


        // Since coordinates can be negative and we use an array
        // we need to have some kind of an offset to index this array
        Vector2 offset = new(minX-gridPadding, minY-gridPadding);

        // Set world data
        wd.Offset = offset;
        wd.Scale = 9;
        wd.Width = gridWidth;
        wd.Height = gridHeight;

        foreach(var l in allLocations)
        {
            // Remove trees
            for (int ix = 0; ix < l.TileWidth; ix++)
            {
                for (int iy = 0; iy < l.TileHeight; iy++)
                {
                    int indexX = (int)(ix + l.X - offset.x);
                    int indexY = (int)(iy + l.Y - offset.y);
                    grid[indexX, indexY] = true;
                }
            }
            l.GenerateLocation(TerrainHolder, wd);
        }

        // Calculate location centers
        List<Vector2> locationCenters = new();
        foreach(var l in allLocations)
        {
            Vector2 center = new Vector2(l.X + (l.TileWidth/2), l.Y + (l.TileHeight / 2));
            locationCenters.Add(center);
        }

        // Create Passageways between locations
        DigOutPaths(locationCenters, grid);

        // Load forest tile
        GameObject[] tiles = Resources.LoadAll<GameObject>("Prefabs/Forest/ForestTiles");

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(grid[x,y]) { continue; }
                Vector3 pos = new Vector3(x + 0.5f, 0, y + 0.5f) * wd.Scale;
                var tile = Instantiate(tiles[0], pos, Quaternion.identity, TerrainHolder.transform);
                
                // MeshRenderer[] meshRenderers = tile.GetComponentsInChildren<MeshRenderer>() ;
                // GameObject[] gameObjects = new GameObject[meshRenderers.Length];
                // for (int i = 0; i < meshRenderers.Length; i++) {
                //     gameObjects[i] = meshRenderers[i].gameObject;
                // }

                // StaticBatchingUtility.Combine(gameObjects, TerrainHolder);
            }
        }

        GenerateMesh(gridWidth, gridHeight);
    }

    private void PlaceLocations(List<Location> locations)
    {
        // Place portal
        Location portal = new ForestPortal();
        portal.SetTileCenter(new(0,0)); // Set it to center
        locations.Add(portal);

        // Medow 
        int mCount = UnityEngine.Random.Range(2, 4);
        for(int i = 0; i < mCount; i++)
        {
            Location medow = new Medow();

            int minRange = 30;
            int maxRange = 60;
            
            int indexX = UnityEngine.Random.Range(minRange, maxRange + 1);
            int indexY = UnityEngine.Random.Range(minRange, maxRange + 1);

            indexX *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            indexY *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

            bool flag = false;
            for(int t = 0; t < 5; t++)
            {
                medow.SetTileCenter(new(indexX, indexY));
                if(medow.CheckLocation(locations)) {
                    flag = true;
                    Debug.Log("Found location");
                    break;
                };
            }

            if(flag) locations.Add(medow);
            else Debug.Log("Didnt found location!");
        }
    }

    #region Mesh

    private void GenerateMesh(int gridWidth, int gridHeight)
    {
        Mesh newMesh = new();
        
        List<Vector3> vertices = new();
        List<int> trianglesFloor = new();
        List<Vector2> uv = new();

        // Generate terrain height (Londek is not helpful)
        float[,] th = new float[gridWidth+1, gridHeight+1];
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++) {
                th[x,y] = UnityEngine.Random.Range(-0.1f, 0.1f);
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
                uv.Add(new (1, 0));
                uv.Add(new (1, 1));
                uv.Add(new (0, 1));

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

        // Get material
        Material dirt = Resources.Load<Material>("Materials/Forest/Dirt/Dirt");

        _mf.mesh = newMesh;
        _mr.materials = new Material[] {dirt};
        _mc.sharedMesh = newMesh;
    }

    #endregion

    private void DigOutPaths(List<Vector2> locationCenters, bool[,] grid) 
    {
        List<Edge> uniqueEdges = new();

        // This is a hack
        // somethimes triangulation does not produce result. In such case, it should be repeated
        do
        {
            // Create "sigma triangle"
            Triangle st = Triangle.GetSuperTriangle(locationCenters);
            Debug.Log("Created sigma triangle: " + st.vA + " " + st.vB + " " + st.vC);

            List<Triangle> triangles = new();
            triangles.Add(st);

            // Triangulate each vertex (magic)
            Debug.Log($"Number of centers: {locationCenters.Count}");
            foreach (var vertex in locationCenters)
            {
                Debug.Log("=== ITERATION ===");
                Debug.Log("Current vertex = " + vertex);
                triangles = AddVertex(vertex, triangles);
            }
            Debug.Log("Number of triangles: " + triangles.Count);

            // Remove triangles that share edges with sigma triangle (they are not so sigma)
            triangles = triangles.Where(t => 
                !(t.vA == st.vA || t.vA == st.vB || t.vA == st.vC ||
                t.vB == st.vA || t.vB == st.vB || t.vB == st.vC ||
                t.vC == st.vA || t.vC == st.vB || t.vC == st.vC)
            ).ToList();

            uniqueEdges = RemoveDuplicateEdges(triangles);
        } while(uniqueEdges.Count == 0);

        #region MST

        Debug.Log($"Number of unique edges: {uniqueEdges.Count}");
        Edge firstEdge = uniqueEdges.Aggregate(uniqueEdges[0], (smallest, next) => {
            return smallest.Length > next.Length ? next : smallest;
        });
        Debug.LogFormat("First shortest edge: {0}-{1}, {2}", firstEdge.v0, firstEdge.v1, firstEdge.Length);
        firstEdge.MarkAsUsed();

        // Do this as long as there are unconnected edges
        // If edge is not connected from both sides, this means that at least one of it's points is not connected
        List<Edge> usedEdges = uniqueEdges.Where(e => e.Used).ToList(); // Used in some later computations
        while(uniqueEdges.FindAll(e => !e.IsConnected(usedEdges)).Count > 0)
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

        // Debug.Log("Dimensions of grid: x" + grid.GetLength(0));
        // Debug.Log("Dimensions of grid: y" + grid.GetLength(1));

        int num = 0;
        foreach(var line in uniqueEdges.Where(e => e.Used))
        {
            num++;
            Vector2 point1 = line.v0.x < line.v1.x ? line.v0 : line.v1;
            Vector2 point2 = line.v0.x > line.v1.x ? line.v0 : line.v1;

            Debug.Log("=== LINE " + num + " ===");
            Debug.Log("First point " + point1);
            Debug.Log("Second point " + point2);

            float a = (point2.y - point1.y)/(point2.x - point1.x);
            // y = ax + b ----> b = y - ax
            float b = point1.y-(a * point1.x);

            int yLength = (int)Math.Abs(Math.Ceiling(a));
            yLength = yLength < 1 ? 1 : yLength; // yLength cannot be smaller than 1
            int ySymbol = a > 0 ? 1 : -1;

            Debug.LogFormat("yLen = {0}, a = {1}, b = {2}", yLength, a, b);
            int thickness = UnityEngine.Random.Range(2, 4);

            for(int ix = (int)Math.Floor(point1.x); ix < (int)Math.Floor(point2.x); ix++)
            {
                int x = ix;
                int indexX = x - (int)wd.Offset.x;

                for(int iy = 0; iy < yLength; iy++)
                {
                    int y = (int)Math.Floor(a*x + b)+(iy*ySymbol);
                    int indexY = y - (int)wd.Offset.y;

                    // Debug.Log("Current x " + x + ", index x " + indexX);
                    // Debug.LogFormat("Actual coordinates: [{0},{1}]", x, y);
                    // Debug.LogFormat("Index: [{0},{1}]", indexX, indexY);
                    for(int tx = 0; tx < thickness; tx++)
                    {
                        for(int ty = 0; ty < thickness; ty++)
                        {
                            grid[indexX+tx,indexY+ty] = true;
                        }
                    }
                }
            }
        }

        num = 0;
        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                if(grid[x,y]) {num++; continue;}
            }
        }
        Debug.Log("Number of not used tiles (inside mst): " + num);

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

    #endregion

    #region Triangulation helper functions
    private List<Triangle> AddVertex(Vector2 vertex, List<Triangle> triangles)
    {
        List<Edge> edges = new();

        // Remove triangles with circumcircles containing the vertex
        triangles = triangles.Where(t => {
            // Debug.Log("triangle center: " + t.vCenter);
            if (t.InCircumcircle(vertex)) {
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
        public Vector2 v0, v1;
        public Edge(Vector2 v0, Vector2 v1)
        {
            this.v0 = v0; this.v1 = v1;
            this.Length = Vector2.Distance(v0, v1);
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
        public Vector2 vA, vB, vC;
        public Vector2 vCenter;
        public Triangle(Vector2 vA, Vector2 vB, Vector2 vC)
        {
            this.vA = vA; this.vB = vB; this.vC = vC;
            this.vCenter = GetCircumcenter(vA, vB, vC);
        }

        public static Triangle GetSuperTriangle(List<Vector2> verticies)
        {
            float minX, minY, maxX, maxY;
            minX = minY = 100000000000; // some big number
            maxX = maxY = -100000000000; // some big number
            foreach(var vertex in verticies)
            {
                minX = Math.Min(minX, vertex.x);
                minY = Math.Min(minX, vertex.y);
                maxX = Math.Max(maxX, vertex.x);
                maxY = Math.Max(maxX, vertex.y);
            }
            var dx = (maxX - minX) * 10;
            var dy = (maxY - minY) * 10;

            var v0 = new Vector2(minX - dx, minY - dy * 3);
            var v1 = new Vector2(minX - dx, maxY + dy);
            var v2 = new Vector2(maxX + dx * 3, maxY + dy);

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
            var radius = Vector2.Distance(vCenter, vA);
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