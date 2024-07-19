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

    public void UpdateBounds()
    {
        minX = 8.4f;
        maxY = 3.02f;
        minY = 0.5f - height - 3.02f;
        maxX = width - 0.5f + 8.4f;
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

    void PanCamera()
    {
        if (!isPanning) return;

        Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition - lastPanPosition);
        Vector3 move = new Vector3(offset.x * panSpeed, offset.y * panSpeed, 0);

        Vector3 newPosition = transform.position - move;

        // Clamp the new position within the defined boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        lastPanPosition = Input.mousePosition;
    }
}
