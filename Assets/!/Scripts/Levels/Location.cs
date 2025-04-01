// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public abstract class LocationOld : MonoBehaviour
// {
//     public string Name;
//     [HideInInspector] public GameObject SpawnedInstance = null;
//     [HideInInspector] public Area LocationRectangle;
//     public abstract void InitLocation(WorldData worldData);

//     #region 2D grid
//     public Vector2 GetTileGridCenter(Vector2 offset)
//     {
//         return LocationRectangle.GetCenter() - offset;
//     }
//     public Vector2 GetTileGridCorner(Vector2 offset)
//     {
//         return LocationRectangle.TopLeftCorner - offset;
//     }
//     public Vector2 GetWorldCenter(Vector2 offset, float scale)
//     {
//         return GetTileGridCenter(offset) * scale;
//     }
//     public Vector2 GetWorldCorner(Vector2 offset, float scale)
//     {
//         return GetTileGridCorner(offset) * scale;
//     }
//     #endregion
//     public Vector2 GetAbsoluteCenter()
//     {
//         return LocationRectangle.GetCenter();
//     }
//     public Vector2 GetAbsoluteCorner()
//     {
//         return LocationRectangle.TopLeftCorner;
//     }
//     public int GetWidth()
//     {
//         return (int)LocationRectangle.Dimensions.x;
//     }
//     public int GetHeight()
//     {
//         return (int)LocationRectangle.Dimensions.y;
//     }

//     public void PlaceInWorld(float scale)
//     {
//         float worldX = (LocationRectangle.TopLeftCorner.x) * scale;
//         float worldY = transform.position.y * scale;
//         float worldZ  = (LocationRectangle.TopLeftCorner.y) * scale;
//         transform.SetPositionAndRotation(new(worldX, worldY, worldZ), Quaternion.identity);
//     }
// }

// public struct Area
// {
//     public Vector2 Dimensions;
//     public Vector2 TopLeftCorner;
//     public Area(Vector2 dimensions)
//     {
//         TopLeftCorner = new(0,0);
//         Dimensions = dimensions;
//     }
//     public Area(Vector2 dimensions, Vector2 topLeftCorner)
//     {
//         TopLeftCorner = topLeftCorner;
//         Dimensions = dimensions;
//     }
//     public Area( float width, float height, float x, float y)
//     {
//         TopLeftCorner = new(x,y);
//         Dimensions = new(width, height);
//     }
//     public static Area AreaFromCenter(Vector2 center, Vector2 dimensions)
//     {
//         Vector2 topLeftCorner = new(center.x - (dimensions.x/2), center.y - (dimensions.y/2));
//         return new
//         (
//             topLeftCorner,
//             dimensions
//         );
//     }

//     // Useful functions
//     public void SetDimensions(float width, float height)
//     {
//         Dimensions = new(width,height);
//     }
//     public void SetCenter(Vector2 center)
//     {
//         TopLeftCorner = center - (Dimensions/2);
//     }
//     public Vector2 GetCenter()
//     {
//         return new(TopLeftCorner.x + (Dimensions.x/2), TopLeftCorner.y + (Dimensions.y/2));
//     }
//     // This function only works, if areas (rectangles) have the same rotation
//     public bool IsOverlapping(Area other)
//     {
//         Vector2 center = GetCenter();
//         Vector2 centerOther = other.GetCenter();

//         float halfWidth = Dimensions.x / 2;
//         float halfHeight = Dimensions.y / 2;

//         float halfWidthOther = other.Dimensions.x / 2;
//         float halfHeightOther = other.Dimensions.y / 2;

//         float diffX = Math.Abs(center.x - centerOther.x);
//         float diffY = Math.Abs(center.y - centerOther.y);

//         float minDiffX = (Dimensions.x/2) + (other.Dimensions.x/2);
//         float minDiffY = (Dimensions.y/2) + (other.Dimensions.y/2);

//         return (diffX < halfWidth + halfWidthOther && diffY < halfHeight + halfHeightOther);
//     }

//     public bool IsOverlapping(Area[] others)
//     {
//         for(int i = 0; i < others.Length; i++)
//         {
//             Area other = others[i];

//             Vector2 center = GetCenter();
//             Vector2 centerOther = other.GetCenter();

//             float halfWidth = Dimensions.x / 2;
//             float halfHeight = Dimensions.y / 2;

//             float halfWidthOther = other.Dimensions.x / 2;
//             float halfHeightOther = other.Dimensions.y / 2;

//             float diffX = Math.Abs(center.x - centerOther.x);
//             float diffY = Math.Abs(center.y - centerOther.y);

//             float minDiffX = (Dimensions.x/2) + (other.Dimensions.x/2);
//             float minDiffY = (Dimensions.y/2) + (other.Dimensions.y/2);

//             if(diffX < halfWidth + halfWidthOther && diffY < halfHeight + halfHeightOther) return true;
//         }
//         return false;
//     }
//     public Vector2[,] GetEdges()
//     {
//         Vector2[,] edges = new Vector2[4,2];

//         Vector2 p1 = new Vector2(TopLeftCorner.x, TopLeftCorner.y);
//         Vector2 p2 = new Vector2(TopLeftCorner.x + Dimensions.x, TopLeftCorner.y);
//         Vector2 p3 = new Vector2(TopLeftCorner.x + Dimensions.x, TopLeftCorner.y + Dimensions.y);
//         Vector2 p4 = new Vector2(TopLeftCorner.x, TopLeftCorner.y + Dimensions.y);

//         edges[0,0] = p1;
//         edges[0,1] = p2;

//         edges[1,0] = p2;
//         edges[1,1] = p3;

//         edges[2,0] = p3;
//         edges[2,1] = p4;

//         edges[3,0] = p4;
//         edges[3,1] = p1;


//         return edges;
//     }
// }

// public class DummyLocation : Location
// {
//     public DummyLocation(Vector2 dimensions, Vector2 position)
//     {
//         Name = "Dummy Location";
//         LocationRectangle = new(dimensions, position);
//     }

//     public override void InitLocation(WorldData worldData)
//     {
        
//     }
// }

// // === Forest ===

// public class ForestBossArena : Location
// {
//     public ForestBossArena()
//     {
//         Name = "Forest Boss Arena";
        
//         LocationRectangle = new(new(11, 11));
//     }

//     public override void InitLocation(WorldData worldData)
//     {
//         GameObject boar = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boss");
//         Vector2 center = GetAbsoluteCenter();

//         GameObject boarInstance = GameObject.Instantiate(boar, new Vector3(center.x, 0.2f, center.y) * worldData.Scale, Quaternion.identity);
   
//         LivingEntity boarLivingEntity = boarInstance.GetComponent<LivingEntity>();
//         boarLivingEntity.OnDeath.AddListener(() => UICanvas.Instance.OpenVideoPlayer());
//     }
// }