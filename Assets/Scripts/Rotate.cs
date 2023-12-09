using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 30f; // Adjust the rotation speed

    void Update()
    {
        // Rotate the image around the Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}