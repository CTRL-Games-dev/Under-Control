// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Unity.Collections;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem.LowLevel;
// using UnityEngine.UIElements;

// public struct Tile
// {
//     public bool IsWall;
//     public int X, Y;
// }

// public struct PathData
// {
//     public Point Point1, Point2;
//     public Quaternion Rotation;
//     public int Thickness;
// }

// public struct Point // Aka vertex
//     {
//         public Vector2 Position;
//         public Location LocationOfPoint;
//         public Point(Vector2 pos, Location location)
//         {
//             Position = pos;
//             LocationOfPoint = location;
//         }
//         public Point(float x, float y, Location location)
//         {
//             Position = new Vector2(x, y);
//             LocationOfPoint = location;
//         }
//         public static bool operator ==(Point a, Point b)
//         {
//             return(a.Position == b.Position);
//         }
//         public static bool operator !=(Point a, Point b)
//         {
//             return(a.Position != b.Position);
//         }
//         public bool Compare(Point other)
//         {
//             return (Position.x == other.Position.x && Position.y == other.Position.y);
//         }
//     } 

// public struct WorldData
// {
//     public Vector2 Offset;
//     public float Scale;
//     public int Width, Height;
//     public bool[,] Grid;
//     public List<Location> Locations;
//     public List<PathData> Paths;
// }

// [RequireComponent(typeof(MeshCollider))]
// [RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(MeshFilter))]
// public class BetterGenerator : MonoBehaviour
// {
//     public WorldData wd;
//     private MeshFilter _meshFilter;
//     private MeshCollider _meshCollider;
//     private MeshRenderer _meshRenderer;
//     public GameObject TerrainHolder;
//     public void Awake()
//     {
//         _meshFilter = GetComponent<MeshFilter>();
//         _meshCollider = GetComponent<MeshCollider>();
//         _meshRenderer = GetComponent<MeshRenderer>();
//     }
//     public enum LevelType
//     {
//         Forest
//     }
//     public void GenerateMap(LevelType type)
//     {
//         switch(type)
//         {
//             case LevelType.Forest: {
//                 GenerateForest();
//             } break;
//         }
//     }

//     public T Getlocation<T>()
//     where T : Location
//     {
//         foreach(var l in wd.Locations)
//         {
//             if(l.GetType() == typeof(T)) return (T)l;
//         }
//         return null;
//     }

//     public List<T> GetAllLocation<T>()
//     where T : Location
//     {
//         List<T> locations = new();
//         foreach(var l in wd.Locations)
//         {
//             if(l.GetType() == typeof(T)) locations.Add((T)l);
//         }
//         return locations;
//     }

//     private void GenerateForest()
//     {
//         wd.Locations = new();
//         wd.Paths = new();

//         PlaceLocations(wd.Locations);

//         // All locations
//         int minX = 0, minY = 0, maxX = 0, maxY = 0;
//         foreach(var l in wd.Locations)
//         {
//             if (l.X < minX) minX = l.X;
//             if (l.Y < minY) minY = l.Y;
//             if (l.X+l.TileWidth > maxX) maxX = l.X+l.TileWidth;
//             if (l.Y+l.TileHeight > maxY) maxY = l.Y+l.TileHeight;

//             Debug.Log("Spawned location at x->" + l.X + " y->" + l.Y);
//         }

//         // Grid is used to determine where to spawn trees
//         int gridPadding = 10;
//         int gridWidth = (int)Vector2.Distance(new(minX,0), new(maxX,0)) + (gridPadding*2);
//         int gridHeight = (int)Vector2.Distance(new(minY,0), new(maxY,0)) + (gridPadding*2);;
//         // int gridHeight = (int)Vector2.Distance(new(0,minY), new(0,maxY));
        
//         bool[,] grid = new bool[gridWidth + (gridPadding), gridHeight + (gridPadding)];


//         // Since coordinates can be negative and we use an array
//         // we need to have some kind of an offset to index this array
//         Vector2 offset = new(minX-gridPadding, minY-gridPadding);

//         // Set world data
//         wd.Offset = offset;
//         wd.Scale = 9;
//         wd.Width = gridWidth;
//         wd.Height = gridHeight;
//         wd.Grid = grid;

//         foreach(var l in wd.Locations)
//         {
//             // Remove trees
//             for (int ix = 0; ix < l.TileWidth; ix++)
//             {
//                 for (int iy = 0; iy < l.TileHeight; iy++)
//                 {
//                     int indexX = (int)(ix + l.X - offset.x);
//                     int indexY = (int)(iy + l.Y - offset.y);
//                     grid[indexX, indexY] = true;
//                 }
//             }
//             l.GenerateLocation(TerrainHolder, wd);
//         }


//         // Create Passageways between locations
//         DigOutPaths(wd.Locations, grid);

//         #region Forest around locations
//         {
//             GameObject[] tiles = Resources.LoadAll<GameObject>("Prefabs/Forest/ForestTiles");
//             foreach(var l in wd.Locations)
//             {
//                 int margin = 5;

//                 int xStart = l.X - (int)(wd.Offset.x) - margin;
//                 int yStart = l.Y - (int)(wd.Offset.y) - margin;

//                 Debug.Log($"xStart = {xStart}, yStart = {yStart} ");

//                 xStart = xStart < 0 ? 0 : xStart;
//                 xStart = xStart > wd.Width ? wd.Width : xStart;

//                 yStart = yStart < 0 ? 0 : yStart;
//                 yStart = yStart > wd.Height ? wd.Height : yStart;

//                 int xEnd = xStart + l.TileWidth + 2 * margin;
//                 int yEnd = yStart + l.TileHeight + 2 * margin;

//                 Debug.Log($"xEnd = {xEnd}, yEnd = {yEnd} ");

//                 xEnd = xEnd < 0 ? 0 : xEnd;
//                 xEnd = xEnd > wd.Width ? wd.Width : xEnd;

//                 yEnd = yEnd < 0 ? 0 : yEnd;
//                 yEnd = yEnd > wd.Height ? wd.Height : yEnd;


//                 Debug.Log($"New xStart = {xStart}, yStart = {yStart} ");
//                 Debug.Log($"New xEnd = {xEnd}, yEnd = {yEnd} ");

//                 for(int x = xStart; x < xEnd; x++)
//                 {
//                     for(int y = yStart; y < yEnd; y++)
//                     {
//                         if(grid[x,y]) { continue; }
//                         Vector3 pos = new Vector3(x + 0.5f, 0, y + 0.5f) * wd.Scale;
//                         var tile = Instantiate(tiles[0], pos, Quaternion.identity, TerrainHolder.transform);
                        
//                         // MeshRenderer[] meshRenderers = tile.GetComponentsInChildren<MeshRenderer>() ;
//                         // GameObject[] gameObjects = new GameObject[meshRenderers.Length];
//                         // for (int i = 0; i < meshRenderers.Length; i++) {
//                         //     gameObjects[i] = meshRenderers[i].gameObject;
//                         // }

//                         // StaticBatchingUtility.Combine(gameObjects, TerrainHolder);
//                     }
//                 }
//             }
//         }

//         #endregion

//         #region Forest around paths

//         {
//             GameObject[] tiles = Resources.LoadAll<GameObject>("Prefabs/Forest/ForestBorder");
//             Debug.Log($"Tiles: {tiles}, {Resources.LoadAll("Prefabs/Forest/ForestBorder")}");
//             foreach(var p in wd.Paths)
//             {
//                 // Length of this path
//                 float len = Vector2.Distance(p.Point1.Position, p.Point2.Position);
                
//                 int numberOfSideTiles = (int)Math.Floor(len-1); // Number of forest tiles on each side
//                 float spacing = wd.Scale;

//                 GameObject pathTileHolder = new GameObject();
//                 pathTileHolder.transform.SetParent(TerrainHolder.transform);

//                 for(int i = 0; i < numberOfSideTiles; i++)
//                 {
//                     Vector3 posLeft = new Vector3(-1f, 0, i) * wd.Scale;
//                     Vector3 posRight = new Vector3(1f, 0, i) * wd.Scale;

//                     GameObject tileLeft = tiles[UnityEngine.Random.Range(0, tiles.Length)];
//                     GameObject tileRight = tiles[UnityEngine.Random.Range(0, tiles.Length)];

//                     Instantiate(tileLeft, posLeft, Quaternion.identity, pathTileHolder.transform);
//                     Instantiate(tileRight, posRight, Quaternion.identity, pathTileHolder.transform);

//                 }
//                 Vector3 pathPos = new Vector3(p.Point1.Position.x - wd.Offset.x, 0, p.Point1.Position.y - wd.Offset.y) * wd.Scale;
//                 Debug.Log($"Position of the path: {pathPos}");
//                 pathTileHolder.transform.rotation = p.Rotation;
//                 pathTileHolder.transform.position = pathPos;
//             }
//         }

//         #endregion

//         GenerateMesh(gridWidth, gridHeight);
//     }

//     private void PlaceLocations(List<Location> locations)
//     {
//         // Place portal
//         Location portal = new ForestPortal();
//         portal.SetTileCenter(new(0,0)); // Set it to center
//         locations.Add(portal);

//         // Medow 
//         int mCount = UnityEngine.Random.Range(2, 4);
//         for(int i = 0; i < mCount; i++)
//         {
//             Location medow = new Medow();

//             int minRange = 30;
//             int maxRange = 60;
            
//             int indexX = UnityEngine.Random.Range(minRange, maxRange + 1);
//             int indexY = UnityEngine.Random.Range(minRange, maxRange + 1);

//             indexX *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
//             indexY *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

//             bool flag = false;
//             for(int t = 0; t < 5; t++)
//             {
//                 medow.SetTileCenter(new(indexX, indexY));
//                 if(medow.CheckLocation(locations)) {
//                     flag = true;
//                     Debug.Log("Found location");
//                     break;
//                 };
//             }

//             if(flag) locations.Add(medow);
//             else Debug.Log("Didnt found location!");
//         }
//     }

//     #region Mesh

//     private void GenerateMesh(int gridWidth, int gridHeight)
//     {
//         Mesh newMesh = new();
        
//         List<Vector3> vertices = new();
//         List<int> trianglesFloor = new();
//         List<Vector2> uv = new();

//         // Generate terrain height
//         float[,] th = new float[gridWidth+1, gridHeight+1];
//         for(int x = 0; x < gridWidth; x++)
//         {
//             for(int y = 0; y < gridHeight; y++) {
//                 th[x,y] = UnityEngine.Random.Range(-0.1f, 0.1f);
//             }
//         }

//         for(int x = 0; x < gridWidth; x++)
//         {
//             for(int y = 0; y < gridHeight; y++) {
//                 int index = x * gridHeight + y;

//                 Vector3 p0 = new Vector3(x,     th[x,y],  y) * wd.Scale;
//                 Vector3 p1 = new Vector3(x,     th[x,y+1],  (y+1)) * wd.Scale;
//                 Vector3 p2 = new Vector3((x+1), th[x+1,y+1],  (y+1)) * wd.Scale;
//                 Vector3 p3 = new Vector3((x+1), th[x+1,y], y) * wd.Scale;

//                 vertices.Add(p0);
//                 vertices.Add(p1);
//                 vertices.Add(p2);
//                 vertices.Add(p3);

//                 uv.Add(new (0, 0));
//                 uv.Add(new (1, 0));
//                 uv.Add(new (1, 1));
//                 uv.Add(new (0, 1));

//                 trianglesFloor.Add(index * 4 + 0);
//                 trianglesFloor.Add(index * 4 + 1);
//                 trianglesFloor.Add(index * 4 + 2);
                
//                 trianglesFloor.Add(index * 4 + 0);
//                 trianglesFloor.Add(index * 4 + 2);
//                 trianglesFloor.Add(index * 4 + 3);
//             }
//         }


//         newMesh.SetVertices(vertices);
//         newMesh.SetTriangles(trianglesFloor, 0);
//         newMesh.SetUVs(0, uv);

//         // Get material
//         Material dirt = Resources.Load<Material>("Materials/Forest/Dirt/Dirt");

//         _meshFilter.mesh = newMesh;
//         _meshRenderer.materials = new Material[] {dirt};
//         _meshCollider.sharedMesh = newMesh;
//     }

//     #endregion

//     private void DigOutPaths(List<Location> locations, bool[,] grid) 
//     {
//         List<Edge> uniqueEdges = new();

//         // This is a hack
//         // somethimes triangulation does not produce result. In such case, it should be repeated
//         // Create "sigma triangle"
//         Triangle st = Triangle.GetSuperTriangle(locations);
//         Debug.Log("Created sigma triangle: " + st.vA + " " + st.vB + " " + st.vC);

//         List<Triangle> triangles = new();
//         triangles.Add(st);

//         // Triangulate each vertex (magic)
//         foreach (var l in locations)
//         {
//             Point vertex = new(l.GetTileCenter(), l);
//             Debug.Log("=== ITERATION ===");
//             Debug.Log("Current vertex (center of location) = " + vertex.Position);
//             triangles = AddVertex(vertex, triangles);
//         }
//         Debug.Log("Number of triangles: " + triangles.Count);

//         // Remove triangles that share edges with sigma triangle (they are not so sigma)
//         triangles = triangles.Where(t => 
//             !(t.vA == st.vA || t.vA == st.vB || t.vA == st.vC ||
//             t.vB == st.vA || t.vB == st.vB || t.vB == st.vC ||
//             t.vC == st.vA || t.vC == st.vB || t.vC == st.vC)
//         ).ToList();

//         uniqueEdges = RemoveDuplicateEdges(triangles);
        

//         #region MST

//         Debug.Log($"Number of unique edges: {uniqueEdges.Count}");
//         Edge firstEdge = uniqueEdges.Aggregate(uniqueEdges[0], (smallest, next) => {
//             return smallest.Length > next.Length ? next : smallest;
//         });
//         Debug.LogFormat("First shortest edge: {0}-{1}, {2}", firstEdge.v0, firstEdge.v1, firstEdge.Length);
//         firstEdge.MarkAsUsed();

//         // Do this as long as there are unconnected edges
//         // If edge is not connected from both sides, this means that at least one of it's points is not connected
//         List<Edge> usedEdges = uniqueEdges.Where(e => e.Used).ToList(); // Used in some later computations
//         while(uniqueEdges.FindAll(e => !e.IsConnected(usedEdges)).Count > 0)
//         {
//             usedEdges = uniqueEdges.Where(e => e.Used).ToList(); // Used in some later computations

//             // Get all edges that are connected to already used edges 
//             // and set sort them in order (so if one is not okay, we can skip it and go to the next one)
//             List<Edge> connectedEdges = uniqueEdges
//                 .FindAll(e => !e.Used && e.IsOnlyPartiallyConnected(usedEdges))
//                 .OrderBy(e => e.Length).ToList();

//             // Debug.Log("=== List of connected edges after sorting ===");
//             foreach(var ce in connectedEdges)
//             {
//                 // Debug.LogFormat("Connected edge: {0}-{1}, {2}", ce.v0, ce.v1, ce.Length);
//             }

//             foreach(var shortest in connectedEdges)
//             {   
//                 // Debug.LogFormat("Next shortest edge: {0}-{1}, {2}", shortest.v0, shortest.v1, shortest.Length);
//                 if(!shortest.IsFullyConnected(usedEdges)) {
//                     shortest.MarkAsUsed();
//                     break;
//                 }
//                 // Debug.Log("Edge was rejected!");
//             }
//         }

//         // Debug.Log("Fully connected edges: " + uniqueEdges.Where(e => e.Used).Count());
//         // foreach(var e in uniqueEdges.Where(e => e.Used))
//         // {
//         //     Debug.LogFormat("vector({0},{1})", e.v0, e.v1);
//         // }
//         #endregion

//         // Small chance for other edges also to be used
//         foreach(var e in uniqueEdges.Where(e => !e.Used))
//         {
//             if(UnityEngine.Random.Range(0, 3) == 1) {
//                 e.MarkAsUsed();
//                 // Debug.LogFormat("Random edge added: ({0})-({1})", e.v0, e.v1);
//             }
//         }

//         #region Rasterization

//         int num = 0;
//         foreach(var line in uniqueEdges.Where(e => e.Used))
//         {
//             num++;
//             Point point1 = line.v0.Position.x < line.v1.Position.x ? line.v0 : line.v1;
//             Point point2 = line.v0.Position.x > line.v1.Position.x ? line.v0 : line.v1;

//             Debug.Log("=== LINE " + num + " ===");
//             Debug.Log("First point " + point1);
//             Debug.Log("Second point " + point2);

//             float a = (point2.Position.y - point1.Position.y)/(point2.Position.x - point1.Position.x);
//             // y = ax + b ----> b = y - ax
//             float b = point1.Position.y-(a * point1.Position.x);

//             int yLength = (int)Math.Abs(Math.Ceiling(a));
//             yLength = yLength < 1 ? 1 : yLength; // yLength cannot be smaller than 1
//             int ySymbol = a > 0 ? 1 : -1;

//             Debug.LogFormat("yLen = {0}, a = {1}, b = {2}", yLength, a, b);
//             int thickness = UnityEngine.Random.Range(2, 4);

//             for(int ix = (int)Math.Floor(point1.Position.x); ix < (int)Math.Floor(point2.Position.x); ix++)
//             {
//                 int x = ix;
//                 int indexX = x - (int)wd.Offset.x;

//                 for(int iy = 0; iy < yLength; iy++)
//                 {
//                     int y = (int)Math.Floor(a*x + b)+(iy*ySymbol);
//                     int indexY = y - (int)wd.Offset.y;

//                     // Debug.Log("Current x " + x + ", index x " + indexX);
//                     // Debug.LogFormat("Actual coordinates: [{0},{1}]", x, y);
//                     // Debug.LogFormat("Index: [{0},{1}]", indexX, indexY);
//                     for(int tx = 0; tx < thickness; tx++)
//                     {
//                         for(int ty = 0; ty < thickness; ty++)
//                         {
//                             grid[indexX+tx,indexY+ty] = true;
//                         }
//                     }
//                 }
//             }

//             PathData path;
//             path.Point1 = point1;
//             path.Point2 = point2;
//             path.Thickness = thickness;
            
//             Vector2 dir = point2.Position - point1.Position; //a vector pointing from pointA to pointB
//             path.Rotation = Quaternion.LookRotation(new(dir.x, 0, dir.y), Vector3.up); //calc a rotation that

//             wd.Paths.Add(path);
//         }

//         #endregion
//     }

//     #region MST helpers

//     private List<Edge> RemoveDuplicateEdges(List<Edge> edges)
//     {
//         List<Edge> uniqueEdges = new();
//         foreach(var e in edges)
//         {
//             if(uniqueEdges.Find(unique => unique.Equals(e)) == null) uniqueEdges.Add(e);
//         }
//         return uniqueEdges;
//     }
//     private List<Edge> RemoveDuplicateEdges(List<Triangle> triangles)
//     {
//         List<Edge> edges = new();
//         triangles.ForEach(t => edges.AddRange(t.GetEdges()));
//         return RemoveDuplicateEdges(edges);
//     }

//     // https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
//     private Vector2 FindIntersection(Edge lineA, Edge lineB, float tolerance = 0.001f)
//     {
//         float x1 = lineA.v0.Position.x, y1 = lineA.v0.Position.y;
//         float x2 = lineA.v1.Position.x, y2 = lineA.v1.Position.y;

//         float x3 = lineB.v0.Position.x, y3 = lineB.v0.Position.y;
//         float x4 = lineB.v1.Position.x, y4 = lineB.v1.Position.y;

//         // equations of the form x=c (two vertical lines) with overlapping
//         if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
//         {
//             throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
//         }

//         //equations of the form y=c (two horizontal lines) with overlapping
//         if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
//         {
//             throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
//         }

//         //equations of the form x=c (two vertical parallel lines)
//         if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
//         {   
//             //return default (no intersection)
//             return default;
//         }

//         //equations of the form y=c (two horizontal parallel lines)
//         if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
//         {
//             //return default (no intersection)
//             return default;
//         }

//         float x, y;

//         if (Math.Abs(x1 - x2) < tolerance)
//         {
//             //compute slope of line 2 (m2) and c2
//             float m2 = (y4 - y3) / (x4 - x3);
//             float c2 = -m2 * x3 + y3;
//             x = x1;
//             y = c2 + m2 * x1;
//         }

//         else if (Math.Abs(x3 - x4) < tolerance)
//         {
//             //compute slope of line 1 (m1) and c2
//             float m1 = (y2 - y1) / (x2 - x1);
//             float c1 = -m1 * x1 + y1;
//             x = x3;
//             y = c1 + m1 * x3;
//         }
//         //lineA & lineB are not vertical 
//         //(could be horizontal we can handle it with slope = 0)
//         else
//         {
//             //compute slope of line 1 (m1) and c2
//             float m1 = (y2 - y1) / (x2 - x1);
//             float c1 = -m1 * x1 + y1;

//             //compute slope of line 2 (m2) and c2
//             float m2 = (y4 - y3) / (x4 - x3);
//             float c2 = -m2 * x3 + y3;

//             //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
//             //plugging x value in equation (4) => y = c2 + m2 * x
//             x = (c1 - c2) / (m2 - m1);
//             y = c2 + m2 * x;

//             //verify by plugging intersection point (x, y)
//             //in orginal equations (1) & (2) to see if they intersect
//             //otherwise x,y values will not be finite and will fail this check
//             if (!(Math.Abs(-m1 * x + y - c1) < tolerance
//                 && Math.Abs(-m2 * x + y - c2) < tolerance))
//             {
//                 //return default (no intersection)
//                 return default;
//             }
//         }

//         //x,y can intersect outside the line segment since line is infinitely long
//         //so finally check if x, y is within both the line segments
//         if (IsInsideLine(lineA, x, y) &&
//             IsInsideLine(lineB, x, y))
//         {
//             return new Vector2 { x = x, y = y };
//         }

//         //return default (no intersection)
//         return default;

//     }

//     // Returns true if given point(x,y) is inside the given line segment
//     private static bool IsInsideLine(Edge line, float x, float y)
//     {
//         return (x >= line.v0.Position.x && x <= line.v1.Position.x
//                     || x >= line.v1.Position.x && x <= line.v0.Position.x)
//                && (y >= line.v1.Position.y && y <= line.v0.Position.y
//                     || y >= line.v1.Position.y && y <= line.v0.Position.x);
//     }

//     #endregion

//     #region Triangulation helper functions
//     private List<Triangle> AddVertex(Point vertex, List<Triangle> triangles)
//     {
//         List<Edge> edges = new();

//         // Remove triangles with circumcircles containing the vertex
//         triangles = triangles.Where(t => {
//             // Debug.Log("triangle center: " + t.vCenter);
//             if (t.InCircumcircle(vertex)) {
//                 // Debug.Log("Bad triangle!");
//                 edges.Add(new Edge(t.vA, t.vB));
//                 edges.Add(new Edge(t.vB, t.vC));
//                 edges.Add(new Edge(t.vC, t.vA));
//                 return false;
//             }
//             return true;
//         }).ToList();
//         // Debug.Log("Number of triangles inside iteration: " + triangles.Count);

//         // Get unique edges
//         List<Edge> uniqueEdges = GetUniqueEdges(edges);

//         foreach(var e in edges)
//         {
//             triangles.Add(new Triangle(e.v0, e.v1, vertex));
//         }

//         return triangles;
//     }
//     private List<Edge> GetUniqueEdges(List<Edge> edges)
//     {
//         List<Edge> uniqueEdges = new();
//         for (var i = 0; i < edges.Count; ++i) {
//             var isUnique = true;

//             // See if edge is unique
//             for (var j = 0; j < edges.Count; ++j) {
//                 if (i != j && edges[i].Equals(edges[j])) {
//                     isUnique = false;
//                     break;
//                 }
//             }

//             // Edge is unique, add to unique edges array
//             if(isUnique) uniqueEdges.Add(edges[i]);
//         }
//         return uniqueEdges;
//     }
//     private List<Edge> GetUniqueEdges(List<Triangle> triangles)
//     {
//         List<Edge> edges = new();
//         triangles.ForEach(t => {
//             edges.Add(new Edge(t.vA, t.vB));
//             edges.Add(new Edge(t.vB, t.vC));
//             edges.Add(new Edge(t.vC, t.vA));
//         });
//         return GetUniqueEdges(edges);
//     }

//     #endregion

//     #region Triangles

//     // === I LOVE TRIANGLES :3 ===
//     // https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/#the-super-triangle
    
//     private class Edge
//     {
//         // Variables and methods used for triangulation
//         public Point v0, v1;
//         public Edge(Point v0, Point v1)
//         {
//             this.v0 = v0; this.v1 = v1;
//             this.Length = Vector2.Distance(v0.Position, v1.Position);
//         }
//         public bool Equals(Edge other)
//         {
//             return (v0.Compare(other.v0) && v1.Compare(other.v1)) || 
//                 (v0.Compare(other.v1) && v1.Compare(other.v0));
//         }

//         // public void MarkAsRedudnant()
//         // {
//         //     NotNeeded = true;
//         // }

//         // Variables and methods used for MST
//         public float Length;
//         public bool Used { get; private set; } = false; // Used to say that this edge is being used

//         // Used to mark this path as no longer useful
//         // public bool NotNeeded { get; private set; } = false ;

//         // This function only checks if other edge is connected with this edge
//         public bool ConnectedWith(Edge other)
//         {
//             return v0.Compare(other.v0) || v1.Compare(other.v1) || 
//                 v0.Compare(other.v1) || v1.Compare(other.v0);
//         }

//         // This function only checks edge is connected at least from one side
//         public bool IsConnected(List<Edge> otherEdges)
//         {
//             bool vA = false, vB = false;
//             foreach(var other in otherEdges)
//             {
//                 if(this == other) continue;
//                 if(v0.Compare(other.v0) || v0.Compare(other.v1)) vA = true;
//                 if(v1.Compare(other.v0) || v1.Compare(other.v1)) vB = true;
//             }
//             // Debug.Log("vA = " + vA + ", vB = " + vB);
//             return vA || vB;
//         }
//         // This function only checks edge is connected only from one side
//         public bool IsOnlyPartiallyConnected(List<Edge> otherEdges)
//         {
//             bool vA = false, vB = false;
//             foreach(var other in otherEdges)
//             {
//                 if(this == other ) continue;
//                 if(v0.Compare(other.v0) || v0.Compare(other.v1)) vA = true;
//                 if(v1.Compare(other.v0) || v1.Compare(other.v1)) vB = true;
//             }
//             // Debug.Log("vA = " + vA + ", vB = " + vB);
//             return vA != vB;
//         }
//         // This function only checks edge is connected from both sides
//         public bool IsFullyConnected(List<Edge> otherEdges)
//         {
//             bool vA = false, vB = false;
//             foreach(var other in otherEdges)
//             {
//                 if(this == other) continue;
//                 if(v0.Compare(other.v0) || v0.Compare(other.v1)) vA = true;
//                 if(v1.Compare(other.v0) || v1.Compare(other.v1)) vB = true;
//             }
//             return vA && vB;
//         }
//         // This function checks if other edge and marks it, if it is
//         public void MarkAsUsed()
//         {
//             Used = true;
//             // NotNeeded = true;
//         }
//     }

//     private class Triangle
//     {
//         public Point vA, vB, vC;
//         public Vector2 vCenter;
//         public Triangle(Point a, Point b, Point c)
//         {
//             this.vA = a; this.vB = b; this.vC = c;
//             this.vCenter = GetCircumcenter(vA, vB, vC);
//         }

//         public static Triangle GetSuperTriangle(List<Location> locations)
//         {
//             float minX, minY, maxX, maxY;
//             minX = minY = 100000000000; // some big number
//             maxX = maxY = -100000000000; // some big number
//             foreach(var l in locations)
//             {
//                 var vertex = l.GetTileCenter();
//                 minX = Math.Min(minX, vertex.x);
//                 minY = Math.Min(minX, vertex.y);
//                 maxX = Math.Max(maxX, vertex.x);
//                 maxY = Math.Max(maxX, vertex.y);
//             }
//             var dx = (maxX - minX) * 10;
//             var dy = (maxY - minY) * 10;

//             var v0 = new Point(minX - dx, minY - dy * 3, new DummyLocation(new(0,0),new(0,0)));
//             var v1 = new Point(minX - dx, maxY + dy, new DummyLocation(new(0,0),new(0,0)));
//             var v2 = new Point(maxX + dx * 3, maxY + dy, new DummyLocation(new(0,0),new(0,0)));

//             return new Triangle(v0, v1, v2);
//         }

//         public List<Edge> GetEdges()
//         {
//             return new()
//             {
//                 new Edge(vA, vB),
//                 new Edge(vB, vC),
//                 new Edge(vC, vA)
//             };
//         }
//         public bool InCircumcircle(Point v)
//         {
//             var dx = vCenter.x - v.Position.x;
//             var dy = vCenter.y - v.Position.y;
//             var radius = Vector2.Distance(vCenter, vA.Position);
//             return Math.Sqrt(dx * dx + dy * dy) <= radius;
//         }

//         // What the fuck is going on?
//         // https://www.reddit.com/r/Unity3D/comments/wppjjd/how_to_calculate_the_circumcenter_of_a_triangle/
//         Vector2 GetCircumcenter(Point vA, Point vB, Point vC)
//         {
//             LinearEquation lineAB = new LinearEquation(vA, vB);
//             LinearEquation lineBC = new LinearEquation(vB, vC);

//             Vector2 midPointAB = Vector2.Lerp(vA.Position, vB.Position, 0.5f);
//             Vector2 midPointBC = Vector2.Lerp(vB.Position, vC.Position, 0.5f);

//             LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
//             LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);

//             return GetCrossingPoint(perpendicularAB, perpendicularBC);
//         }
//         private class LinearEquation 
//         {
//             public float _A, _B, _C;
//             public LinearEquation() {}
//             public LinearEquation(Point vA, Point vB)
//             {
//                 float deltaX = vB.Position.x - vA.Position.x;
//                 float deltaY = vB.Position.y - vA.Position.y;
//                 _A = deltaY;
//                 _B = -deltaX;
//                 _C = _A * vA.Position.x + _B * vA.Position.y;
//             }

//             public LinearEquation PerpendicularLineAt(Vector3 point)
//             {
//                 LinearEquation newLine = new LinearEquation();
//                 newLine._A = -_B;
//                 newLine._B = _A;
//                 newLine._C = newLine._A * point.x + newLine._B * point.y;
//                 return newLine;
//             }
//         }
//         Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
//         {
//             float A1 = line1._A;
//             float A2 = line2._A;
//             float B1 = line1._B;
//             float B2 = line2._B;
//             float C1 = line1._C;
//             float C2 = line2._C;


//             // Cramer's rule
//             float Determinant = A1 * B2 - A2 * B1;
//             float DeterminantX = C1 * B2 - C2 * B1;
//             float DeterminantY = A1 * C2 - A2 * C1;

//             float x = DeterminantX / Determinant;
//             float y = DeterminantY / Determinant;
//             return new Vector2(x, y);
//         }
//     }

//     #endregion
// }