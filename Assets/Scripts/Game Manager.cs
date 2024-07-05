using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform levelViewTransform;
    [SerializeField] private List<GameObject> Tiles;

    private string LevelName = "Test";
    private int Width = 17, Height = 7;
    private string SavePath;

    private Dictionary<int, Vector3> RoadRotations;
    private Dictionary<Vector2, int> RoadOrientations;
    private TileType selectedTileType;

    #region Unity

    void Start()
    {
        Tiles = new List<GameObject>();
        RoadOrientations = new Dictionary<Vector2, int>();

        if (PlayerPrefs.HasKey("SavePath"))
        {
            SavePath = PlayerPrefs.GetString("SavePath");
            Debug.Log("Save path loaded: " + SavePath);
        }

        // Create preset rotations for the 3D Tile Object
        InstantiateRoadRotations();

        // If theres a level load it, if not create one
        
        if (File.Exists(SavePath))
        {
            LoadLevel();
        }
        else
        {
            GenerateGrid();
        }
    }

    private void OnApplicationQuit()
    {
        SaveLevel();
    }

    #endregion

    #region File Handling

    private void SaveLevel()
    {
        // Saves name and dimentions
        File.WriteAllText(SavePath, "");
        File.AppendAllText(SavePath, LevelName + "\r\n");
        File.AppendAllText(SavePath, Width.ToString() + "," + Height.ToString() + "\r\n");

        // Saves each tile in "Type/Oriention" format with "," as seperator
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                TileType currentTileType = Tiles[(y * this.Width) + x].GetComponent<Tile>().tileType;
                int currentOrientation = RoadOrientations.Single(v => v.Key == new Vector2(x, y)).Value;

                File.AppendAllText(SavePath, currentTileType.ToString() + "/" + currentOrientation.ToString() + ",");
            }
            File.AppendAllText(SavePath, "\r\n");
        }
    }

    private void LoadLevel()
    {
        string[] levelData = File.ReadAllLines(SavePath);

        // Assign name and dimentions
        this.LevelName = levelData[0];
        this.Width = int.Parse(levelData[1].Split(",")[0]);
        this.Width = int.Parse(levelData[1].Split(",")[1]);

        // Read rest of file which the data for the tiles
        int listIndex = 0;
        for (int y = 0; y < this.Height; y++)
        {
            string[] rowData = levelData[y + 2].Split(",");

            for (int x = 0; x < this.Width; x++)
            {
                string[] tileData = rowData[x].Split("/");

                Tiles.Add(Instantiate(tilePrefab, new Vector2(x, -y), Quaternion.identity));
                Tiles[listIndex].name = $"{x},{y}";

                TileType currentTileType = new TileType();
                switch (tileData[0])
                {
                    case "Grass":
                        currentTileType = TileType.Grass;
                        break;
                    case "Building":
                        currentTileType = TileType.Building;
                        break;
                    case "Road":
                        currentTileType = TileType.Road;
                        break;
                };

                Tiles[listIndex].GetComponent<Tile>().tileType = currentTileType;

                RoadOrientations.Add(new Vector2(x, y), int.Parse(tileData[1]));               

                if (currentTileType == TileType.Grass)
                {
                    ChangeTileOrientation(x, y, 0, new Vector3(0, 0, 0));
                }
                else if (currentTileType == TileType.Building)
                {
                    ChangeTileOrientation(x, y, 0, new Vector3(180, 0, 0));
                }
                else if (currentTileType == TileType.Road)
                {
                    Vector3 roadRotation = RoadRotations.Single(v => v.Key == int.Parse(tileData[1])).Value;
                    ChangeTileOrientation(x, y, int.Parse(tileData[1]), roadRotation);
                }

                listIndex++;
            }
        }

        levelViewTransform.transform.position = new Vector3((float)Width / 2 - 0.5f, -2.75f, -10);
    }

    #endregion

    #region Tile Changing

    public void ChangeTile (int x, int y)
    {
        // If placing tile is not a road tile, change it then return
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
        TileType neighbouringTileType;

        // Checks each neighbouring tile veritcally and horizontically, if its a road then update it
        if (y + 1 < this.Height) // Up + y
        {
            neighbouringTileType = Tiles[((y + 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                UpdateRoad(x, y + 1);
            }
        }
        if (x + 1 < this.Width) // Right + x
        {
            neighbouringTileType = Tiles[(y * this.Width) + x + 1].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                UpdateRoad(x + 1, y);
            }
        }
        if (y - 1 >= 0) // Down - y
        {
            neighbouringTileType = Tiles[((y - 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                UpdateRoad(x, y - 1);
            }
        }
        if (x - 1 >= 0) // Left - x
        {
            neighbouringTileType = Tiles[(y * this.Width) + x - 1].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                UpdateRoad(x - 1, y);
            }
        }
    }

    public void UpdateRoad (int x, int y)
    {
        ChangeTileType(TileType.Road, x, y);
        Vector3 rotation = new Vector3(0, 0, 0);

        int rotationIndex = 1;
        TileType neighbouringTileType;

        // Check neighbouring tiles, multiply rotation index accordingly
        if (y - 1 >= 0) // Up - y
        {
            neighbouringTileType = Tiles[((y - 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                rotationIndex *= 2;
            }
        }
        if (x + 1 < this.Width) // Right + x
        {
            neighbouringTileType = Tiles[(y * this.Width) + x + 1].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                rotationIndex *= 3;
            }
        }
        if (y + 1 < this.Height) // Down + y
        {
            neighbouringTileType = Tiles[((y + 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
            {
                rotationIndex *= 4;
            }
        }
        if (x - 1 >= 0) // Left - x
        {
            neighbouringTileType = Tiles[(y * this.Width) + x - 1].GetComponent<Tile>().tileType;
            if (neighbouringTileType == TileType.Road)
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
        RoadOrientations[new Vector2(x, y)] = key;

        Transform tileTransform = Tiles[(y * this.Width) + x].transform;

        // Reset 3D tile rotation, apply x,y rotation on 3D, apply z rotation on 2D
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

    #endregion

    #region UI

    public void ReturnToLevelSelector ()
    {
        SaveLevel();
        SceneManager.LoadScene("LevelSelector");
    }

    #endregion

    private void InstantiateRoadRotations()
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

    private void GenerateGrid()
    {
        // Generates grid of grass tile
        int listIndex = 0;
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                Tiles.Add(Instantiate(tilePrefab, new Vector2(x, -y), Quaternion.identity));

                Tiles[listIndex].name = $"{x},{y}";
                RoadOrientations.Add(new Vector2(x, y), 0);

                listIndex++;
            }
        }
        levelViewTransform.transform.position = new Vector3((float)Width / 2 - 0.5f, -2.75f, -10);
    }
}
