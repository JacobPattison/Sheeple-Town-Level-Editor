using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    public float panSpeed = 20f;

    // Define boundaries
    public float minX = -0.5f;
    public float maxX = 16.5f;
    public float minY = -0.5f;
    public float maxY = 8.5f;

    public static int width;
    public static int height;

    private Vector3 lastPanPosition;
    private bool isPanning;

    private void Start()
    {
        maxX = width - 0.5f;
        maxY = height - 0.5f;

        // Manual Offset 
        minX += 8.377266f;
        maxX += 8.377266f;
        minY += -3.021386f;
        maxY += -3.021386f;
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

        // Debugging
        Debug.Log($"Camera Position: {transform.position}");
        Debug.Log($"Pan Move: {move}");
    }
}
