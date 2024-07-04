using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class GameManager : MonoBehaviour
{
    [SerializeField] private string LevelName = "Test";
    [SerializeField] private int Width, Height;
    private string SavePath = Application.dataPath + "/Levels/Test.txt";
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform levelViewTransform;
    [SerializeField] private List<GameObject> Tiles;
    [SerializeField] private Dictionary<int, Vector3> RoadRotations;
    [SerializeField] private Dictionary<Vector2, int> RoadOrientations;
    [SerializeField] private TileType selectedTileType;

    void Start()
    {
        Tiles = new List<GameObject>();
        RoadOrientations = new Dictionary<Vector2, int>();
        //GenerateGrid();
        InstantiateRoadRotations();
        LoadLevel();

        if (!File.Exists(SavePath))
        {
            File.WriteAllText(SavePath, "");
        }

        Debug.Log(SavePath);
    }

    private void OnApplicationQuit()
    {
        //SaveLevel();
    }

    private void SaveLevel()
    {
        File.WriteAllText(SavePath, "");
        File.AppendAllText(SavePath, LevelName + "\r\n");
        File.AppendAllText(SavePath, Width.ToString() + "," + Height.ToString() + "\r\n");

        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                TileType tileType = Tiles[(y * this.Width) + x].GetComponent<Tile>().tileType;
                int orientation = RoadOrientations.Single(v => v.Key == new Vector2(x, y)).Value;

                File.AppendAllText(SavePath, tileType.ToString() + "/" + orientation.ToString() + ",");
            }
            File.AppendAllText(SavePath, "\r\n");
        }
    }

    private void LoadLevel()
    {
        string[] levelData = File.ReadAllLines(SavePath);

        this.LevelName = levelData[0];
        this.Width = int.Parse(levelData[1].Split(",")[0]);
        this.Width = int.Parse(levelData[1].Split(",")[1]);

        int counter = 0;
        for (int y = 0; y < this.Height; y++)
        {
            string[] rowContents = levelData[y + 2].Split(",");

            foreach (string rowContent in rowContents)
                Debug.Log(rowContent);

            for (int x = 0; x < this.Width; x++)
            {
                string[] tileContents = rowContents[x].Split("/");

                Debug.Log(tileContents[0] + tileContents[1]);

                Tiles.Add(Instantiate(tilePrefab, new Vector2(x, -y), Quaternion.identity));
                Tiles[counter].name = $"{x},{y}";

                TileType tileType = new TileType();

                switch (tileContents[0])
                {
                    case "Grass":
                        tileType = TileType.Grass;
                        break;
                    case "Building":
                        tileType = TileType.Building;
                        break;
                    case "Road":
                        tileType = TileType.Road;
                        break;
                };

                Tiles[counter].GetComponent<Tile>().tileType = tileType;

                RoadOrientations.Add(new Vector2(x, y), int.Parse(tileContents[1]));               

                if (tileType == TileType.Grass)
                {
                    ChangeTileOrientation(x, y, 0, new Vector3(0, 0, 0));
                }
                else if (tileType == TileType.Building)
                {
                    ChangeTileOrientation(x, y, 0, new Vector3(180, 0, 0));
                }
                else if (tileType == TileType.Road)
                {
                    Vector3 rotation = RoadRotations.Single(v => v.Key == int.Parse(tileContents[1])).Value;
                    ChangeTileOrientation(x, y, int.Parse(tileContents[1]), rotation);
                }

                counter++;
            }
        }
        levelViewTransform.transform.position = new Vector3((float)Width / 2 - 0.5f, -2.75f, -10);
    }

    private void InstantiateRoadRotations ()
    {
        RoadRotations = new Dictionary<int, Vector3>();

        // Zero Roads
        RoadRotations.Add(1, new Vector3(90, 0, 0));

        // One Road
        RoadRotations.Add(15, new Vector3(90, 0, 0));
        RoadRotations.Add(3, new Vector3(90, 0, 0));
        RoadRotations.Add(5, new Vector3(90, 0, 0));

        RoadRotations.Add(8, new Vector3(90, 0, 90));
        RoadRotations.Add(2, new Vector3(90, 0, 90));
        RoadRotations.Add(4, new Vector3(90, 0, 90));

        // Two Roads
        RoadRotations.Add(6, new Vector3(90, 270, 0));
        RoadRotations.Add(12, new Vector3(90, 270, -90));
        RoadRotations.Add(20, new Vector3(90, 270, -180));
        RoadRotations.Add(10, new Vector3(90, 270, -270));

        // Three Roads
        RoadRotations.Add(30, new Vector3(90, 180, 0));
        RoadRotations.Add(24, new Vector3(90, 180, -90));
        RoadRotations.Add(60, new Vector3(90, 180, -180));
        RoadRotations.Add(40, new Vector3(90, 180, -270));

        // Four Roads
        RoadRotations.Add(120, new Vector3(0, 90, 0));

    }

    private void GenerateGrid ()
    {
        int counter = 0;
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                Tiles.Add(Instantiate(tilePrefab, new Vector2(x, -y), Quaternion.identity));
                Tiles[counter].name = $"{x},{y}";
                RoadOrientations.Add(new Vector2(x, y), 0);
                counter++;
            }
        }
        levelViewTransform.transform.position = new Vector3((float)Width / 2 - 0.5f, -2.75f, -10);
    }

    public void ChangeTile (int x, int y)
    {
        if (selectedTileType == TileType.Grass)
        {
            ChangeTileType(selectedTileType, x, y);
            ChangeTileOrientation(x, y, 0, new Vector3(0, 0, 0));
            return;
        }
        else if (selectedTileType == TileType.Building)
        {
            ChangeTileType(selectedTileType, x, y);
            ChangeTileOrientation(x, y, 0, new Vector3(180, 0, 0));
            return;
        }
        
        UpdateRoad(x, y);
        if (y + 1 < this.Height) // Up + y
        {
            TileType tileType = Tiles[((y + 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                UpdateRoad(x, y + 1);
            }
        }
        if (x + 1 < this.Width) // Right + x
        {
            TileType tileType = Tiles[(y * this.Width) + x + 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                UpdateRoad(x + 1, y);
            }
        }
        if (y - 1 >= 0) // Down - y
        {
            TileType tileType = Tiles[((y - 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                UpdateRoad(x, y - 1);
            }
        }
        if (x - 1 >= 0) // Left - x
        {
            TileType tileType = Tiles[(y * this.Width) + x - 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                UpdateRoad(x - 1, y);
            }
        }
    }

    public void UpdateRoad (int x, int y)
    {
        Debug.Log($"{x}, {y}");

        ChangeTileType(TileType.Road, x, y);
        Vector3 rotation = new Vector3(0, 0, 0);
        int rotationIndex = 1;
        if (y - 1 >= 0) // Up - y
        {
            TileType tileType = Tiles[((y - 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 2;
            }
        }
        if (x + 1 < this.Width) // Right + x
        {
            TileType tileType = Tiles[(y * this.Width) + x + 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 3;
            }
        }
        if (y + 1 < this.Height) // Down + y
        {
            TileType tileType = Tiles[((y + 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 4;
            }
        }
        if (x - 1 >= 0) // Left - x
        {
            TileType tileType = Tiles[(y * this.Width) + x - 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 5;
            }
        }

        int key = 0;

        foreach (KeyValuePair<int, Vector3> item in RoadRotations)
        {
            if (item.Key == rotationIndex)
            {
                rotation = item.Value;
                key = item.Key;
            }
        }

        ChangeTileOrientation(x, y, key,  rotation);
    }

    private void ChangeTileType(TileType tileType, int x, int y)
    {
        int listIndex = y * this.Width + x;
        Tile tileComponent = Tiles[listIndex].GetComponent<Tile>();
        tileComponent.tileType = tileType;
    }

    private void ChangeTileOrientation (int x, int y, int key, Vector3 rotation)
    {
        Transform tileTransform = Tiles[(y * this.Width) + x].transform;

        RoadOrientations[new Vector2(x, y)] = key;


        foreach (Transform childTranform in tileTransform.transform)
        {
            childTranform.transform.eulerAngles = new Vector3(0, 0, 0);
            tileTransform.transform.eulerAngles = new Vector3(0, 0, 0);

            childTranform.transform.eulerAngles = new Vector2(rotation.x, rotation.y);
            tileTransform.transform.eulerAngles = new Vector3(0f, 0f, rotation.z);
        }
    }

    public void ChangeSelectedTileType(int type)
    {
        switch (type)
        {
            case 0:
                this.selectedTileType = TileType.Grass;
                return;
            case 1:
                this.selectedTileType = TileType.Building;
                return;
            case 2:
                this.selectedTileType = TileType.Road;
                return;
        };
    }
}
