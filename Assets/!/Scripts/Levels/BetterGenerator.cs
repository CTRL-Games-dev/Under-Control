using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public struct Tile
    {
        public bool IsWall;
        public int X, Y;
    }

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BetterGenerator : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("starting deneration");
        GenerateMap(LevelType.Forest);
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

    private void GenerateForest()
    {
        List<Location> allLocations = new();
        List<Location> generatedLocations = new();

        // All locations
        allLocations.Add(new ForestPortal());
        allLocations.Add(new DummyLocation(new(1,1), new(3,3)));
        allLocations.Add(new DummyLocation(new(1,1), new(3,-3)));
        allLocations.Add(new DummyLocation(new(1,1), new(-3,-3)));
        allLocations.Add(new DummyLocation(new(1,1), new(-3,3)));

        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        foreach(var l in allLocations)
        {
            l.FindLocation(generatedLocations);
            l.GenerateLocation();
            generatedLocations.Add(l);

            if (l.X < minX) minX = l.X;
            if (l.Y < minY) minY = l.Y;
            if (l.X+l.Width > maxX) maxX = l.X+l.Width;
            if (l.Y+l.Height > maxY) maxY = l.Y+l.Height;

            Debug.Log("Spawned location at x->" + l.X + " y->" + l.Y);
        }



        // Grid is used to determine where to spawn trees
        int gridPadding = 5;
        int gridWidth = (int)Vector2.Distance(new(minX,0), new(maxX,0)) + (gridPadding*2);
        int gridHeight = gridWidth;
        // int gridHeight = (int)Vector2.Distance(new(0,minY), new(0,maxY));
        
        bool[,] grid = new bool[gridWidth + (gridPadding*2), gridHeight + (gridPadding*2)];
        List<Vector2> locationCenters = new();

        // Since coordinates can be negative and we use an array
        // we need to have some kind of an offset to index this array
        Vector2 offset = new(minX-gridPadding, minY-gridPadding);

        // Remove trees from places where locations are
        // and calculate their centers in the meantime
        foreach(var l in generatedLocations)
        {
            for (int ix = 0; ix < l.Width; ix++)
            {
                for (int iy = 0; iy < l.Height; iy++)
                {
                    int indexX = (int)(ix + l.X - offset.x);
                    int indexY = (int)(iy + l.Y - offset.y);
                    grid[indexX, indexY] = true;
                }
            }
            Vector2 center = new(l.X + (l.Width/2), l.Y + (l.Height / 2));
            locationCenters.Add(center);
        }

        // Create Passageways between locations
        DigOutPaths(locationCenters, grid, offset);

        // Load forest tile
        GameObject[] tile = Resources.LoadAll<GameObject>("Prefabs/Forest/ForestTiles");
        int num = 0;
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(grid[x,y]) {num++; continue;}
                GameObject.Instantiate(tile[0], new(x + 0.5f, 0, y + 0.5f), Quaternion.identity);
            }
        }
        Debug.Log("Number of not used tiles: " + num);
    }

    private void DigOutPaths(List<Vector2> locationCenters, bool[,] grid, Vector2 gridOffset) 
    {
        // Create "sigma triangle"
        Triangle st = Triangle.GetSuperTriangle(locationCenters);
        // Debug.Log("Created sigma triangle: " + st.vA + " " + st.vB + " " + st.vC);

        List<Triangle> triangles = new();
        triangles.Add(st);

        // Triangulate each vertex (magic)
        foreach (var vertex in locationCenters)
        {
            // Debug.Log("=== ITERATION ===");
            // Debug.Log("Current vertex = " + vertex);
            triangles = AddVertex(vertex, triangles);
        }
        // Debug.Log("Number of triangles: " + triangles.Count);

        // Remove triangles that share edges with sigma triangle (they are not so sigma)
        triangles = triangles.Where(t => 
            !(t.vA == st.vA || t.vA == st.vB || t.vA == st.vC ||
            t.vB == st.vA || t.vB == st.vB || t.vB == st.vC ||
            t.vC == st.vA || t.vC == st.vB || t.vC == st.vC)
        ).ToList();

        // === MST ===

        List<Edge> uniqueEdges = RemoveDuplicateEdges(triangles);

        #region MST
        Debug.Log("Number of unique edges" + uniqueEdges.Count);
        foreach(var e in uniqueEdges)
        {
            Debug.LogFormat("Unique edge: {0}-{1}, {2}", e.v0, e.v1, e.Length);
        }

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

            Debug.Log("=== List of connected edges after sorting ===");
            foreach(var ce in connectedEdges)
            {
                Debug.LogFormat("Connected edge: {0}-{1}, {2}", ce.v0, ce.v1, ce.Length);
            }

            foreach(var shortest in connectedEdges)
            {   
                Debug.LogFormat("Next shortest edge: {0}-{1}, {2}", shortest.v0, shortest.v1, shortest.Length);
                if(!shortest.IsFullyConnected(usedEdges)) {
                    shortest.MarkAsUsed();
                    break;
                }
                Debug.Log("Edge was rejected!");
            }
        }

        Debug.Log("Fully connected edges: " + uniqueEdges.Where(e => e.Used).Count());
        foreach(var e in uniqueEdges.Where(e => e.Used))
        {
            Debug.LogFormat("vector({0},{1})", e.v0, e.v1);
        }
        #endregion

        // Small chance for other edges also to be used
        foreach(var e in uniqueEdges.Where(e => !e.Used))
        {
            if(UnityEngine.Random.Range(0, 3) == 1) {
                e.MarkAsUsed();
                Debug.LogFormat("Random edge added: ({0})-({1})", e.v0, e.v1);
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

            for(int ix = (int)Math.Floor(point1.x); ix < (int)Math.Floor(point2.x); ix++)
            {
                int x = ix;
                int indexX = x - (int)gridOffset.x;

                for(int iy = 0; iy < yLength; iy++)
                {
                    int y = (int)Math.Floor(a*x + b)+(iy*ySymbol);
                    int indexY = y - (int)gridOffset.y;

                    // Debug.Log("Current x " + x + ", index x " + indexX);
                    Debug.LogFormat("Actual coordinates: [{0},{1}]", x, y);
                    Debug.LogFormat("Index: [{0},{1}]", indexX, indexY);
                    grid[indexX,indexY] = true;
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