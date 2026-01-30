using UnityEngine;

public class LocomotionPC : LocomotionBase
{
    [SerializeField] private Camera pcCamera;

    void Update()
    {
        if (!attachedA) UpdateRayVisual(leftLine);
        if (!attachedB) UpdateRayVisual(rightLine);

        // Left Click = Anchor A, Right Click = Anchor B
        if (Input.GetMouseButtonDown(0)) ShootRay(ref attachedA, ref anchorPointA, 0);
        if (Input.GetMouseButtonDown(1)) ShootRay(ref attachedB, ref anchorPointB, 1);

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

    void ShootRay(ref bool isAttached, ref Vector3 anchorPoint, int rayId)
    {
        if (!isAttached)
        {
            Vector3 worldOrigin = pcCamera.transform.position;
            Vector3 dir = pcCamera.transform.rotation * Vector3.forward;

            if (Physics.Raycast(worldOrigin, dir, out RaycastHit hit, maxDistance, combinedMask))
            {
                anchorPoint = hit.point;
                isAttached = true;

                AttachGrapple(rayId, hit.point);
            }
        }
        else isAttached = false;
    }

    void UpdateRayVisual(LineRenderer line)
    {
        if (line == null) return;

        Vector3 worldOrigin = pcCamera.transform.position;
        Vector3 dir = pcCamera.transform.rotation * Vector3.forward;

        line.SetPosition(0, worldOrigin);

        if (Physics.Raycast(worldOrigin, dir, out RaycastHit hit, maxDistance, combinedMask))
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, worldOrigin + (dir * maxDistance));
        }
    }
}