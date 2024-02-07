using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float zoomSpeed = 0.5f;  // Adjust the zoom speed
    public float minZoom = 1.0f;    // Minimum zoom level
    public float maxZoom = 10.0f;   // Maximum zoom level 

    private Camera camera;
    private float initialPinchDistance;
    private float initialOrthoSize;

    void Start()
    {
        camera = Camera.main;
        initialPinchDistance = 0f;
        initialOrthoSize = camera.orthographicSize;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            // Calculate the pinch distance between two touches
            float pinchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

            // If it's the first frame of the pinch, record the initial pinch distance
            if (initialPinchDistance == 0f)
            {
                initialPinchDistance = pinchDistance;
                initialOrthoSize = camera.orthographicSize;
            }

            // Calculate the zoom amount based on the change in pinch distance
            float zoomAmount = (pinchDistance - initialPinchDistance) * zoomSpeed;

            // Calculate the new orthographic size
            float newOrthoSize = initialOrthoSize - zoomAmount;

            // Clamp the orthographic size to the specified range
            newOrthoSize = Mathf.Clamp(newOrthoSize, minZoom, maxZoom);

            // Apply the new orthographic size to the camera
            camera.orthographicSize = newOrthoSize;
        }
        else
        {
            // Reset initial values when not zooming
            initialPinchDistance = 0f;
            initialOrthoSize = camera.orthographicSize;
        }
    }
}