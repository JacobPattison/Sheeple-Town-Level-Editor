using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private List<GameObject> Tiles;
    [SerializeField] private Dictionary<int, Vector3> RoadRotations;

    void Start()
    {
        Tiles = new List<GameObject>();
        GenerateGrid();
        InstantiateRoadRotations();
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
                counter++;
            }
        }
        cameraTransform.transform.position = new Vector3((float)Width / 2 - 0.5f, -(float)Height / 2 + 0.5f, -10);

        Debug.Log(Tiles.Count);
    }

    public void ChangeTile (int x, int y)
    {
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
                //Debug.Log("Up: " + tileType);
            }
        }
        if (x + 1 < this.Width) // Right + x
        {
            TileType tileType = Tiles[(y * this.Width) + x + 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 3;
                //Debug.Log("Right: " + tileType);
            }
        }
        if (y + 1 < this.Height) // Down + y
        {
            TileType tileType = Tiles[((y + 1) * this.Width) + x].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 4;
                //Debug.Log("Down: " + tileType);
            }
        }
        if (x - 1 >= 0) // Left - x
        {
            TileType tileType = Tiles[(y * this.Width) + x - 1].GetComponent<Tile>().tileType;
            if (tileType == TileType.Road)
            {
                rotationIndex *= 5;
                //Debug.Log("Left: " + tileType);
            }
        }

        foreach (KeyValuePair<int, Vector3> item in RoadRotations)
        {
            if (item.Key == rotationIndex)
            {
                //Debug.Log(item.Value.x.ToString() + ", " + item.Value.y.ToString() + ", " + item.Value.z.ToString());
                rotation = item.Value;
            }
        }
        
        Transform tileTransform = Tiles[(y * this.Width) + x].transform;

        foreach(Transform childTranform in tileTransform.transform)
        {
            childTranform.transform.eulerAngles = new Vector3 (0, 0, 0);
            tileTransform.transform.eulerAngles = new Vector3(0, 0, 0);

            childTranform.transform.eulerAngles = new Vector2(rotation.x, rotation.y);
            tileTransform.transform.eulerAngles = new Vector3(0f, 0f, rotation.z);
        }

        Debug.Log($"Road Updated to {Tiles[(y * this.Width) + x]} at {x}, {y}");
    }

    private void ChangeTileType(TileType tileType, int x, int y)
    {
        int listIndex = y * this.Width + x;
        Tile tileComponent = Tiles[listIndex].GetComponent<Tile>();
        tileComponent.tileType = tileType;
    }
}
