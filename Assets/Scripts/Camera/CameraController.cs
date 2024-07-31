using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2.0f;
    public float moveSpeed = 5.0f;
    
    [SerializeField] private Camera _MainCamera;

    public Vector2 XMovementClamps; //X represents the min, Y represents the max.
    public Vector2 YMovementClamps; //X represents the min, Y represents the max.
    
    private Vector3 touchStart;
    private float initialZoomDistance;

    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            float zoomInput = Input.GetAxis("Mouse ScrollWheel");
            Zoom(zoomInput);

            if (Input.GetMouseButton(1))
            {
                Move();
            }
        }

        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    initialZoomDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float currentZoomDistance = Vector2.Distance(touch1.position, touch2.position);
                    float deltaZoom = currentZoomDistance - initialZoomDistance;
                    Zoom(-deltaZoom * zoomSpeed);
                    initialZoomDistance = currentZoomDistance;
                }
            }

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    touchStart = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 swipeDelta = _MainCamera.ScreenToWorldPoint(touchStart) - _MainCamera.ScreenToWorldPoint(touch.position);
                    swipeDelta.z = 0;
                    transform.Translate(swipeDelta * moveSpeed * Time.deltaTime);
                    touchStart = touch.position;
                }
            }
        }
    }

    void Zoom(float deltaZoom)
    {
        _MainCamera.orthographicSize = Mathf.Clamp(_MainCamera.orthographicSize - deltaZoom * zoomSpeed, 5f, 15f);
    }

    void Move()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        
        
        Vector3 moveDirection = new Vector3(-mouseX, -mouseY, 0);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        
        float clampedX = Mathf.Clamp(transform.position.x, XMovementClamps.x, XMovementClamps.y);
        float clampedY = Mathf.Clamp(transform.position.y, YMovementClamps.x, YMovementClamps.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
