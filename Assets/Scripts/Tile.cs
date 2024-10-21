using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] public TileType tileType;

    void Start()
    {
        GameObject gameManagerFind = GameObject.Find("Game Manager");
        gameManager = gameManagerFind.GetComponent<GameManager>();
    }

    // When mouse is pressed the tile coordinates is passed to the GM to change the tile
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            string[] tileCoordinates = this.gameObject.name.Split(',');
            int x = int.Parse(tileCoordinates[0]);
            int y = int.Parse(tileCoordinates[1]);

            gameManager.ChangeTile(x, y);
            Debug.Log($"{x}, {y}");
        }
    }
}