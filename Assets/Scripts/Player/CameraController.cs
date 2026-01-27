using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate vertical rotation (up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Calculate horizontal rotation (left/right)
        yRotation += mouseX;

        // Apply rotation to the camera (vertical) and player (horizontal)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
