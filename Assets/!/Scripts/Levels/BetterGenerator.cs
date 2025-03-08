using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

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
        }

        // Grid is used to determine where to spawn trees
        int gridWidth = (int)Vector2.Distance(new(minX,0), new(maxX,0));
        int gridHeight = (int)Vector2.Distance(new(0,minY), new(0,maxY));
        bool[,] grid = new bool[gridWidth, gridHeight];
        List<Vector2> locationCenters = new();

        // Since coordinates can be negative and we use an array
        // we need to have some kind of an offset to index this array
        Vector2 offset = new(minX, minY);

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
        DigOutPaths(locationCenters);
    }

    private void DigOutPaths(List<Vector2> locationCenters) 
    {
        // Create "sigma triangle"
        Triangle st = Triangle.GetSuperTriangle(locationCenters);

        List<Triangle> triangles = new();
        triangles.Add(st);

        // Triangulate each vertex (magic)
        foreach (var vertex in locationCenters)
        {
            triangles = AddVertex(vertex, triangles);
        }

        // Remove triangles that share edges with sigma triangle (they are not so sigma)
        triangles = triangles.Where(t => 
            !(t.vA == st.vA || t.vA == st.vB || t.vA == st.vC ||
            t.vB == st.vA || t.vB == st.vB || t.vB == st.vC ||
            t.vC == st.vA || t.vC == st.vB || t.vC == st.vC)
        ).ToList();

        // === MST ===

        List<Edge> uniqueEdges = GetUniqueEdges(triangles);

        // foreach(var e in uniqueEdges)
        // {
        //     Vertex vA = null, vB = null;
        //     // Checking if vertex already exists
        //     // if so, assign it
        //     foreach(var v in vertices)
        //     {
        //         if(e.v0 == v.pos) vA = v;
        //         if(e.v1 == v.pos) vB = v;
        //     }
        //     // If vertex doesn't exist, create it
        //     vA ??= new Vertex(e.v0);
        //     vB ??= new Vertex(e.v1);
        //     // Create new edge (so edgy)
        //     mstEdges.Add(new MSTEdge(vA, vB));
        // }


        Debug.Log("Nugger " + uniqueEdges.Count);
        Edge firstEdge = uniqueEdges.Aggregate(uniqueEdges[0], (smallest, next) => {
            return smallest.Length > next.Length ? next : smallest;
        });
        firstEdge.MarkAsUsed();

        // Do this as long as there are unconnected edges
        // If edge is not connected from both sides, this means that at least one of it's points is not connected
        while(uniqueEdges.FindAll(e => e.IsFullyConnected(uniqueEdges)).Count > 0)
        {
            // Get all edges that are connected to already used edges 
            // and set sort them in order (so if one is not okay, we can skip it and go to the next one)
            List<Edge> connectedEdges = uniqueEdges
                .FindAll(e => !e.Used || e.IsOnlyPartiallyConnected(uniqueEdges))
                .OrderBy(e => e.Length).ToList();

            foreach(var shortest in connectedEdges)
            {
                // Check if it's points aren't already connected
                bool flag = true;
                foreach(var e in uniqueEdges.FindAll(e => shortest.ConnectedWith(e)))
                {
                    // If at least one of the other edges connected to this new edge
                    // this means that new point is already connected, so skip
                    if(e.Used) {
                        flag = false;
                        break;
                    }
                }

                // If shortest edge acually connected new point, then mark it as used
                if(flag) shortest.MarkAsUsed();
            }
        }
    }

    #region MST helpers

    // class Vertex 
    // {
    //     public Vector2 pos;
    //     private bool _visited;
    //     public Vertex(Vector2 pos)
    //     {
    //         this.pos = pos;
    //     }
    //     // Returns true, if was never visited
    //     public bool SetVisited()
    //     {
    //         if(_visited) return false;
    //         _visited = true;
    //         return true;
    //     }
    //     public bool WasVisited()
    //     {
    //         return _visited;
    //     }
    // }

    // class MSTEdge
    // {
    //     public Vertex vA, vB;
    //     public float Value;
        
    //     public MSTEdge(Vertex vA, Vertex vB)
    //     {
    //         this.vA = vA;
    //         this.vB = vB;

    //         this.Value = Vector2.Distance(vA.pos, vB.pos);
    //     }
    // }

    #endregion

    #region Triangulation helper functions
    private List<Triangle> AddVertex(Vector2 vertex, List<Triangle> triangles)
    {
        List<Edge> edges = new();

        // Remove triangles with circumcircles containing the vertex
        triangles = triangles.Where(t => {
            if (t.InCircumcircle(vertex)) {
                edges.Add(new Edge(t.vA, t.vB));
                edges.Add(new Edge(t.vB, t.vC));
                edges.Add(new Edge(t.vC, t.vA));
                return false;
            }
            return true;
        }).ToList();

        // Get unique edges
        List<Edge> uniqueEdges = GetUniqueEdges(edges);

        foreach(var e in edges)
        {
            triangles.Add(new Triangle(e.v0, e.v1, vertex));
        }

        return triangles;
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

        // This function only checks if other edge is connected with this edge
        public bool ConnectedWith(Edge other)
        {
            return v0 == other.v0 || v1 == other.v1 || 
                v0 == other.v1 || v1 == other.v0;
        }
        // This function only checks edge is connected at least from one side
        public bool IsOnlyPartiallyConnected(List<Edge> otherEdges)
        {
            bool vA = false, vB = false;
            foreach(var other in otherEdges)
            {
                if(other.v0 == v0 || other.v1 == v0) vA = true;
                if(other.v0 == v1 || other.v1 == v1) vB = true;
            }
            return vA != vB;
        }
        // This function only checks edge is connected from both sides
        public bool IsFullyConnected(List<Edge> otherEdges)
        {
            bool vA = false, vB = false;
            foreach(var other in otherEdges)
            {
                if(other.v0 == v0 || other.v1 == v0) vA = true;
                if(other.v0 == v1 || other.v1 == v1) vB = true;
            }
            return vA && vB;
        }
        // This function checks if other edge and marks it, if it is
        public void MarkAsUsed()
        {
            Used = true;
            NotNeeded = true;
        }

        public void MarkAsRedudnant()
        {
            NotNeeded = true;
        }

        // Variables and methods used for MST
        public float Length;
        public bool Used { get; private set; } = false; // Used to say that this edge is being used
        // Used to mark this path as no longer useful
        public bool NotNeeded { get; private set; } = false ;
    }

    private class Triangle
    {
        public Vector2 vA, vB, vC;
        Vector2 vCenter;
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
            LinearEquation perpendicularBC = lineAB.PerpendicularLineAt(midPointBC);

            return GetCrossingPoint(perpendicularAB, perpendicularBC);
        }

        Vector2 GetCrossingPoint(LinearEquation lineA, LinearEquation lineB)
        {
            // Cramer's rule
            float Determinant = lineA._A * lineB._B - lineB._A * lineA._B;
            float DeterminantX = lineA._C * lineB._B - lineB._C * lineA._B;
            float DeterminantY = lineA._A * lineB._C - lineB._A * lineA._C;

            float x = DeterminantX / Determinant;
            float y = DeterminantY / Determinant;
            return new Vector2(x, y);
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

            public LinearEquation PerpendicularLineAt(Vector3 v)
            {
                LinearEquation newLine = new LinearEquation();
                newLine._A = -_B;
                newLine._B = _A;
                newLine._C = newLine._A * v.x + newLine._B * v.y;
                return newLine;
            }
        }
    }

    #endregion
}