
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CellType
{

}

public class WorldGenerator : MonoBehaviour {
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
    public void GenerateMap(Dimension type)
    {

        Vector2 gridDimensions;
        CellType[,] grid;

        switch(type)
        {
            case Dimension.FOREST: {
                gridDimensions = PlaceLocationsForest();
            } break;
            default: {
                Debug.LogError($"This type of dimension is not implemented! -> {type}");
            } break;
        }
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
    public (List<Location>, Vector2) PlaceLocationsForest()
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

        List<Location> locations = nodes.Select(x => x.Location).ToList();

        return (locations, new(maxX - minX, maxY - minY));
    }

    #endregion
}