using UnityEngine;

public class LocomotionPC : LocomotionBase
{
    [SerializeField] private Camera pcCamera;

    void Update()
    {
        // Left Click = Anchor A, Right Click = Anchor B
        if (Input.GetMouseButtonDown(0)) ShootRay(ref attachedA, anchorPointA);
        if (Input.GetMouseButtonDown(1)) ShootRay(ref attachedB, anchorPointB);

        // Space = Jump/Fly
        if (Input.GetKeyDown(KeyCode.Space)) ExecuteJump();

        // Shift = Pulling (Grappling)
        ApplyGrappleLogic(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftShift));

        // WASD Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = pcCamera.transform.rotation * new Vector3(h, 0, v) * moveSpeed;
        rb.AddForce(moveDir, ForceMode.Acceleration);

        ApplyVelocityConstraints();
    }

    void ShootRay(ref bool isAttached, GameObject visual)
    {
        if (!isAttached)
        {
            Ray ray = pcCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, combinedMask))
            {
                visual.transform.position = hit.point;
                visual.SetActive(true);
                isAttached = true;
            }
        }
        else { visual.SetActive(false); isAttached = false; }
    }
}