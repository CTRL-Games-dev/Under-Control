using System.Collections.Generic;

public interface IBiome
{
    public void GenerateBiome(Tile[] biomeTiles);
    public void GenerateBiome(List<Tile> biomeTiles);
}