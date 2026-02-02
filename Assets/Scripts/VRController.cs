using UnityEngine;

public class VRController : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] OVRInput.Controller controller;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] GameObject environment;
    [SerializeField] float rotationSensitivity = 100f; // Adjust for feel

    int currentSelectedWall = -1;
    bool isWallGrabbed = false;

    Vector3 initialPos = Vector3.zero;

    private void Update()
    {
        // 1. Initial Grab
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            SelectWall();
        }

        // 2. Dragging/Rotating
        if (isWallGrabbed && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            RotateEnvironment();
        }

        // 3. Release
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            isWallGrabbed = false;
            currentSelectedWall = -1;
        }

        Vector3 dir = transform.rotation * OVRInput.GetLocalControllerRotation(controller) * Vector3.forward;
        Vector3 origin = OVRInput.GetLocalControllerPosition(controller) + transform.position;
        line.SetPosition(0, origin + Vector3.down);
        line.SetPosition(1, origin + (dir * 50));

}

void SelectWall()
    {
        Vector3 dir = transform.rotation * OVRInput.GetLocalControllerRotation(controller) * Vector3.forward;

        if (Physics.Raycast(
            OVRInput.GetLocalControllerPosition(controller) + transform.position + Vector3.down,
            dir,
            out RaycastHit hit,
            Mathf.Infinity,
            wallLayer))
        {

            GameObject wall = hit.collider.gameObject;
            string wallName = wall.name;

            Debug.Log(wallName);
            // Map wall names to IDs
            if (wallName.Contains("North")) currentSelectedWall = 0;
            else if (wallName.Contains("South")) currentSelectedWall = 1;
            else if (wallName.Contains("West")) currentSelectedWall = 2;
            else if (wallName.Contains("East")) currentSelectedWall = 3;

            if (currentSelectedWall != -1)
            {
                isWallGrabbed = true;
                initialPos = OVRInput.GetLocalControllerPosition(controller);
            }
        }
    }

    void RotateEnvironment()
    {
        Vector3 currentPos = OVRInput.GetLocalControllerPosition(controller);
        Vector3 offset = currentPos - initialPos;
        float rotationAmount = 0f;

        // Determine rotation based on which wall is held
        // We use different axes of the controller movement depending on the wall orientation
        switch (currentSelectedWall)
        {
            case 0: // North: Moving hand Left/Right (X) rotates the room
                rotationAmount = offset.z * rotationSensitivity;
                environment.transform.Rotate(Vector3.right, +rotationAmount);
                break;
            case 1: // South
                rotationAmount = offset.z * rotationSensitivity;
                environment.transform.Rotate(Vector3.right, -rotationAmount);
                break;

            case 2: // West: Moving hand Forward/Back (Z) rotates the room
                rotationAmount = offset.z * rotationSensitivity;
                environment.transform.Rotate(Vector3.forward, +rotationAmount);
                break;
            case 3: // East
                rotationAmount = offset.z * rotationSensitivity;
                environment.transform.Rotate(Vector3.forward, -rotationAmount);
                break;
        }

        // Apply the rotation to the environment around the Y axis

        // Update initialPos to the current frame to make the movement "incremental"
        // This prevents the rotation from accelerating exponentially
        initialPos = currentPos;
    }
}