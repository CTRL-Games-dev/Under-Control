using System;
using System.Collections.Generic;
using UnityEngine;

public class BetterGenerator
{
    public struct Tile
    {
        public bool IsWall;
        public int X, Y;
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
        int gridHeight = (int)Vector2.Distance(new(minX,0), new(maxX,0));
        int[,] grid = new int[Math.Abs(minX)+];
        foreach(var l in generatedLocations)
        {

        }
    }
}