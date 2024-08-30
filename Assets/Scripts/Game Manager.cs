using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject TempRoad;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject GridPrefab;
    [SerializeField] private GameObject LevelView;

    [SerializeField] private GameObject EditorUI;
    [SerializeField] private GameObject RoutesUI;

    [SerializeField] private Transform UITransform;

    [SerializeField] private Camera ThumbnailCamera;

    private List<GameObject> Tiles;
    private List<GameObject> Grids;

    private string LevelName;
    private string SavePath;
    private int Width, Height;
    private bool GridActive;
    private bool IsTest;
    public bool IsMoveable;
    public bool IsEditor = true;
    private bool IsPlacingStart;

    private Dictionary<int, Vector3> PresetRoadRotations;
    private Dictionary<Vector2, int> RoadOrientations;

    private TileType selectedTileType;

    Routes Routes;

    #region Unity

    void Start()
    {
        Tiles = new List<GameObject>();
        RoadOrientations = new Dictionary<Vector2, int>();
        Grids = new List<GameObject>();
        this.GridActive = true;

        // Create preset rotations for the 3D Tile Object
        InstantiatePresetRoadRotations();

        // If theres a level load it, if not create one
        CheckExistingLevel();

        // Generate the outline for the grid
        GenerateGridOutline();

        // Toggle grid off by default
        ToggleGridOutline();

        // Set dimentions for the level view
        LevelView.GetComponent<LevelView>().width = this.Width;
        LevelView.GetComponent<LevelView>().height = this.Height;
        LevelView.GetComponent<LevelView>().UpdateBounds();

        Routes = new Routes();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        SaveLevel();
    }


    #endregion

    #region File Handling

    private void SaveLevel()
    {
        if (IsTest) return;

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
            if ((y == this.Height - 1) == false)
                File.AppendAllText(SavePath, "\r\n");
        }

        PlayerPrefs.SetString("IsLoadingNewLevelUI", "False");

    }

    private void LoadLevel()
    {
        string[] levelData = File.ReadAllLines(SavePath);

        // Assign name and dimentions
        this.LevelName = levelData[0];
        this.Width = int.Parse(levelData[1].Split(",")[0]);
        this.Height = int.Parse(levelData[1].Split(",")[1]);

        // Read rest of file which the data for the tiles
        int listIndex = 0;
        for (int y = 0; y < this.Height; y++)
        {
            string[] rowData = levelData[y + 2].Split(",");

            for (int x = 0; x < this.Width; x++)
            {
                string[] tileData = rowData[x].Split("/");

                Tiles.Add(Instantiate(tilePrefab, new Vector3(x, -y, 90), Quaternion.identity));
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
                    Vector3 roadRotation = PresetRoadRotations.Single(v => v.Key == int.Parse(tileData[1])).Value;
                    ChangeTileOrientation(x, y, int.Parse(tileData[1]), roadRotation);
                }

                listIndex++;
            }
        }
    }

    private void SaveThumbnail()
    {
        if (IsTest) return;

        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        ThumbnailCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        ThumbnailCamera.Render();

        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;

        string imageSavePath = Path.GetDirectoryName(SavePath);

        byte[] byteArray = renderedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(imageSavePath + "/Thumbnails/" + this.LevelName + ".png", byteArray);
    }

    #endregion

    #region Instantiates

    private void CheckExistingLevel()
    {
        if (PlayerPrefs.HasKey("SavePath"))
        {
            IsTest = false;
            SavePath = PlayerPrefs.GetString("SavePath");
            Debug.Log("Save path loaded: " + SavePath);

            if (File.Exists(SavePath))
            {
                LoadLevel();
            }
            else
            {
                this.LevelName = PlayerPrefs.GetString("levelName");
                this.Width = int.Parse(PlayerPrefs.GetString("levelWidth"));
                this.Height = int.Parse(PlayerPrefs.GetString("levelHeight"));
                GenerateGrid();
            }
        }
        else
        {
            IsTest = true;
            this.LevelName = "Test";
            this.Width = 17;
            this.Height = 9;
            GenerateGrid();
        }
    }

    private void InstantiatePresetRoadRotations()
    {
        PresetRoadRotations = new Dictionary<int, Vector3>();

        // Zero Roads
        PresetRoadRotations.Add(1, new Vector3(90, 0, 0));

        // One Road
        PresetRoadRotations.Add(15, new Vector3(90, 0, 0));
        PresetRoadRotations.Add(3, new Vector3(90, 0, 0));
        PresetRoadRotations.Add(5, new Vector3(90, 0, 0));

        PresetRoadRotations.Add(8, new Vector3(90, 0, 90));
        PresetRoadRotations.Add(2, new Vector3(90, 0, 90));
        PresetRoadRotations.Add(4, new Vector3(90, 0, 90));

        // Two Roads
        PresetRoadRotations.Add(6, new Vector3(90, 270, 0));
        PresetRoadRotations.Add(12, new Vector3(90, 270, -90));
        PresetRoadRotations.Add(20, new Vector3(90, 270, -180));
        PresetRoadRotations.Add(10, new Vector3(90, 270, -270));

        // Three Roads
        PresetRoadRotations.Add(30, new Vector3(90, 180, 0));
        PresetRoadRotations.Add(24, new Vector3(90, 180, -90));
        PresetRoadRotations.Add(60, new Vector3(90, 180, -180));
        PresetRoadRotations.Add(40, new Vector3(90, 180, -270));

        // Four Roads
        PresetRoadRotations.Add(120, new Vector3(0, 90, 0));

    }

    private void GenerateGrid()
    {
        // Generates grid of grass tile
        int listIndex = 0;
        for (int y = 0; y < this.Height; y++)
        {

            for (int x = 0; x < this.Width; x++)
            {
                Tiles.Add(Instantiate(tilePrefab, new Vector3(x, -y, 90), Quaternion.identity));

                Tiles[listIndex].name = $"{x},{y}";
                RoadOrientations.Add(new Vector2(x, y), 0);

                listIndex++;
            }
        }
    }

    private void GenerateGridOutline()
    {
        int vertLines = this.Width - 1;
        int horiLines = this.Height - 1;

        float verticalOffSet = 0;
        float horizontalOffSet = 0;

        if (this.Height % 2 == 0)
            verticalOffSet = 0.5f;

        if (this.Width % 2 == 0)
            horizontalOffSet = 0.5f;

        for (int x = 0; x < vertLines; x++)
        {
            GameObject grid = Instantiate(GridPrefab, new Vector3(0.5f + x, verticalOffSet - (this.Height / 2), 85f), Quaternion.identity);
            grid.transform.localScale = new Vector3(0.05f, this.Height);

            Grids.Add(grid);
        }

        for (int y = 0; y < horiLines; y++)
        {
            GameObject grid = Instantiate(GridPrefab, new Vector3(-(horizontalOffSet - (this.Width / 2)), -(0.5f + y), 85f), Quaternion.identity);
            grid.transform.localScale = new Vector3(this.Width, 0.05f);

            Grids.Add(grid);
        }
    }


    #endregion

    #region Tile Changing

    public void ChangeTile (int x, int y)
    {
        TileType currentTileType = Tiles[((y) * this.Width) + x].GetComponent<Tile>().tileType;

        if (!IsEditor)
        {
            if (currentTileType == TileType.Road)
                PlacePoints(x, y);
            return;
        }

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
        foreach (KeyValuePair<int, Vector3> item in PresetRoadRotations)
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

    public void ReturnToLevelSelector()
    {
        SaveThumbnail();
        SaveLevel();
        SceneManager.LoadScene("LevelSelector");
    }

    public void CreateNewLevelUI()
    {
        SaveThumbnail();
        SaveLevel();
        PlayerPrefs.SetString("IsLoadingNewLevelUI", "True");
        SceneManager.LoadScene("LevelSelector");
    }

    public void Exit()
    {
        SaveThumbnail();
        SaveLevel();
        UnityEditor.EditorApplication.ExitPlaymode();
        Application.Quit();
    }
    public void ToggleGridOutline()
    {
        if (GridActive)
        {
            foreach (GameObject gridOutline in Grids)
            {
                gridOutline.SetActive(false);
            }
            GridActive = false;
        }
        else
        {
            foreach (GameObject gridOutline in Grids)
            {
                gridOutline.SetActive(true);
            }
            GridActive = true;
        }
    }


    #endregion

    #region Routes Input

    List<GameObject> TempRoads;

    private void PlacePoints (int x, int y)
    {
        if (IsPlacingStart)
        {
            Routes.StartX = x;
            Routes.StartY = y;
            GameObject.Find("Start").transform.position = new Vector3(x, -y, 88);
        }
        else
        {
            Routes.EndX = x;
            Routes.EndY = y;
            GameObject.Find("End").transform.position = new Vector3(x, -y, 88);
        }
    }

    public void ToggleRoutes()
    {
        if (IsEditor)
        {
            EditorUI.SetActive(false);
            RoutesUI.SetActive(true);
            IsEditor = false;
        }
        else
        {
            EditorUI.SetActive(true);
            RoutesUI.SetActive(false);
            IsEditor = true;
        }
    }

    public void StartRoute()
    {
        IsPlacingStart = true;
    }

    public void EndRoute()
    {
        IsPlacingStart = false;
    }

    public void ShowShortestPaths()
    {
        
    }

    public void ShowLongestPaths()
    {

    }

    public void ShowAllPaths()
    {
        Routes.AbstractRoadTiles(this.Tiles, this.Height, this.Width);
        DebugRoadGrid(this.Height, this.Width);
    }

    public void DebugRoadGrid(int Height, int Width)
    {
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                if (Routes.RoadGrid[x, y] == 1)
                {
                    GameObject Road = Instantiate(TempRoad, new Vector3(x, -y), Quaternion.identity);
                    Road.GetComponent<SpriteRenderer>().color = Color.grey;
                    TempRoads.Add(Road);
                }
                else
                {
                    GameObject Road = Instantiate(TempRoad, new Vector3(x, -y), Quaternion.identity);
                    Road.GetComponent<SpriteRenderer>().color = Color.green;
                    TempRoads.Add(Road);
                }
            }
        }
    }

    #endregion
}