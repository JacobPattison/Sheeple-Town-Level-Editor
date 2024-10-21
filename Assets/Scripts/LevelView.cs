using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    public int width;
    public int height;

    public float panSpeed = 10f; // Speed at which the camera moves
    public float minX, maxX; // Minimum and maximum X bounds
    public float minY, maxY; // Minimum and maximum Y bounds

    private bool isPanning = false;
    private Vector3 lastPanPosition;

    // Calulates the bounds for the camera using the level dimentions and view dimentions
    public void UpdateBounds()
    {
        minX = 8.4f;
        minY = -3.02f;

        if (width > 17)
            maxX = minX + width - 17.8f; // 17.8 is the level view width
        else
        { 
            maxX = minX;
            transform.position = new Vector3((float)width / 2 - 0.5f, transform.position.y, transform.position.z);
        }

        if (height > 8)
            maxY = minY - height + 8.52f; // 8.52 is the level view height
        else
        {
            maxY = minY;
            transform.position = new Vector3(transform.position.x, (float)height / 2 - 0.5f, transform.position.z);
        }
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // Start panning
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
        }

        // Continue panning
        if (Input.GetMouseButton(2))
        {
            PanCamera();
        }
        else
        {
            isPanning = false;
        }
    }

    // Moves camera based on mouse movements
    void PanCamera()
    {
        if (!isPanning) return;

        Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition - lastPanPosition);
        Vector3 move = new Vector3(offset.x * panSpeed, offset.y * panSpeed, 0);

        Vector3 newPosition = transform.position - move;

        // Clamp the new position within the defined boundaries
        //if (width <= 17)
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        //if (height <= 8)
            newPosition.y = Mathf.Clamp(newPosition.y, maxY, minY);

        transform.position = newPosition;

        lastPanPosition = Input.mousePosition;
    }
}
