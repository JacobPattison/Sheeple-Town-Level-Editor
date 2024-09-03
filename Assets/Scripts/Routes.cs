using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Routes
{
    public int[,] RoadGrid { get; set; }
    private List<int[,]> Paths;
    public int StartX { private get; set; }
    public int StartY { private get; set; }
    public int EndX { private get; set; }
    public int EndY { private get; set; }

    public void AbstractRoadTiles(List<GameObject> Tiles, int Height, int Width)
    {
        RoadGrid = new int[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                TileType currentTileType = Tiles[(y * Width) + x].GetComponent<Tile>().tileType;

                if (currentTileType == TileType.Road)
                {
                    RoadGrid[x, y] = 1;
                }
            }
        }
    }

    private static readonly int[][] Directions = new int[][]
    {
        new int[] {-1, 0},  // up
        new int[] {1, 0},   // down
        new int[] {0, -1},  // left
        new int[] {0, 1}    // right
    };

    public List<int[,]> CalculatePaths()
    {
        int rows = this.RoadGrid.GetLength(0);
        int cols = this.RoadGrid.GetLength(1);

        // Initialize the path matrix with 0s
        int[,] currentPath = new int[rows, cols];
        List<int[,]> allPaths = new List<int[,]>();

        // Function to check if a cell is within bounds and is a road
        bool IsValid(int x, int y)
        {
            return x >= 0 && x < rows && y >= 0 && y < cols && this.RoadGrid[x, y] == 1;
        }

        // The DFS function
        void DFSRecursive(int x, int y)
        {
            // If we reach the end, add the current path to allPaths
            if (x == this.EndX && y == this.EndY)
            {
                currentPath[x, y] = 1;
                int[,] pathCopy = new int[rows, cols];
                Array.Copy(currentPath, pathCopy, currentPath.Length);
                allPaths.Add(pathCopy);
                currentPath[x, y] = 0; // backtrack
                return;
            }

            // Mark the current cell as visited in the path
            currentPath[x, y] = 1;

            // Explore all four directions
            foreach (var direction in Directions)
            {
                int newX = x + direction[0];
                int newY = y + direction[1];

                // Check if the new position is valid and not yet visited
                if (IsValid(newX, newY) && currentPath[newX, newY] == 0)
                {
                    DFSRecursive(newX, newY);
                }
            }

            // Backtrack: Unmark the current cell
            currentPath[x, y] = 0;
        }

        // Start the DFS from the start position
        if (!IsValid(this.StartX, this.StartY) || !IsValid(this.EndX, this.EndY))
        {
            Console.WriteLine("Invalid start or end position.");
            return allPaths;
        }

        DFSRecursive(this.StartX, this.StartY);

        this.Paths = allPaths;
        return allPaths;
    }
}
