using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 500f; // Speed of camera movement
    public float zoomSpeed = 1f;     // Speed of camera zoom
    public float minZoom = -23f;     // Minimum zoom distance
    public float maxZoom = -10f;      // Maximum zoom distance
    public float minY = 1f;          // Minimum height
    public float maxY = 20f;         // Maximum height
    public float rotationSpeed = 3f; // Speed of camera rotation
    public float minRotationX = -90f; // Minimum rotation around X axis
    public float maxRotationX = 90f;  // Maximum rotation around X axis

    void Update()
    {
        // Camera Movement
        float verticalInput = Input.GetAxis("Mouse ScrollWheel");
        float zoomInput = Input.GetAxis("Vertical");
        float mouseY = Input.GetAxis("Mouse Y");



        Vector3 movement = new Vector3(0f, 0f, verticalInput) * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);

        // Camera Zoom
        float newZoom = transform.position.z + zoomInput * zoomSpeed;
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZoom);

     
            // Clamp camera's position along Y-axis to prevent going beyond minY and maxY
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);

        // Camera Rotation
        if (Input.GetMouseButton(1)) // Right mouse button pressed
        {
            float newRotationX = transform.eulerAngles.x - mouseY * rotationSpeed;
            newRotationX = Mathf.Clamp(newRotationX, minRotationX, maxRotationX);
            transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    }
    

