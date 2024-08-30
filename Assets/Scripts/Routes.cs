using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routes
{
    public int[,] RoadGrid { get; set; }

    public int StartX { private get; set; }
    public int StartY { private get; set; }
    public int EndX { private get; set; }
    public int EndY { private get; set; }

    public void AbstractRoadTiles(List<GameObject> Tiles, int Height, int Width)
    {
        RoadGrid = new int[Height, Width];

        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                TileType currentTileType = Tiles[(y * Width) + x].GetComponent<Tile>().tileType;

                if (currentTileType == TileType.Road)
                {
                    RoadGrid[x, y] = 1;
                }
            }
        }
    }

    public void CalculatePaths()
    {
        
    }

}
