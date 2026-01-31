using UnityEngine;

public class LocomotionPC : LocomotionBase
{
    [SerializeField] private Camera pcCamera;

    protected override void Update()
    {
        base.Update();

        Vector3 camPos = pcCamera.transform.position;
        Vector3 camDir = pcCamera.transform.forward;

        // Visuals
        if (!attachedA) UpdateRayVisual(leftLine, camPos, camDir);
        if (!attachedB) UpdateRayVisual(rightLine, camPos, camDir);

        // Shooting
        if (Input.GetMouseButtonDown(0)) HandleGrappleToggle(0, camPos, camDir);
        if (Input.GetMouseButtonDown(1)) HandleGrappleToggle(1, camPos, camDir);

        // Movement
        ApplyGrappleLogic(Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftShift));

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = Vector3.ProjectOnPlane(pcCamera.transform.rotation * new Vector3(h, 0, v), Vector3.up).normalized;
        rb.AddForce(moveDir * moveSpeed, ForceMode.Acceleration);

        if (Input.GetKeyDown(KeyCode.Space)) ExecuteJump();

        ApplyVelocityConstraints();
    }
}