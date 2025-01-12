using System.Text;
using UnityEngine;
public class MapGenerator
{
    public enum Tile
    {
        WALL,
        FLOOR,
    }
    public Tile[,] GetMap(int width, int height, int iterations)
    {
        Tile[,] grid = GetNoise(width, height);

        for(int i = 0; i < iterations; i++)
        {
            Tile[,] temp_grid = CopyGrid(grid);
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    temp_grid[x,y] = CheckTile(grid, x, y);
                }
            }
            grid = temp_grid;
        }

        return grid;
    }

    private Tile CheckTile(Tile[,] grid, int x, int y)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int wall_count = 0;
        for(int ix = -1; ix <= 1; ix++)
        {
            for(int iy = -1; iy <= 1; iy++)
            {
                if(iy == 0 && ix == 0)
                    continue;
                    

                int indexX = x + ix;
                int indexY = y + iy;

                if(indexX >= width || indexX < 0) { wall_count++; continue; }
                if(indexY >= height || indexY < 0) { wall_count++; continue; }
                if(grid[indexX, indexY] == Tile.WALL) { wall_count++; continue; }
            }
        }
        Debug.Log(wall_count);
        if(wall_count >= 4) 
            return Tile.WALL;
        else
            return Tile.FLOOR;
    }

    private Tile[,] GetNoise(int width, int height)
    {
        Tile[,] tiles = new Tile[width, height];
        float density = 20;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float chance = Random.Range(0, 100);
                if(chance >= density)
                    tiles[x,y] = Tile.FLOOR;
                else
                    tiles[x,y] = Tile.WALL;
            }
        }

        return tiles;
    }

    private Tile[,] CopyGrid(Tile[,] grid)
    {
        Tile[,] temp_grid = new Tile[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                temp_grid[i, j] = grid[i, j];
            }
        }
        return temp_grid;
    }
}
