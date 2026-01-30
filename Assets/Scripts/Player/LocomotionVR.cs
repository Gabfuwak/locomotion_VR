using UnityEngine;

public class LocomotionVR : LocomotionBase
{
    [SerializeField] private Transform eyeTransform;


    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;

    void Update()
    {
        // Display Rays to see where you shoot
        if (!attachedA) UpdateRayVisual(leftLine, leftController);
        if (!attachedB) UpdateRayVisual(rightLine, rightController);

        // Handle Shooting Grapples
        HandleRaycast(OVRInput.Button.PrimaryIndexTrigger, leftController, ref attachedA, ref anchorPointA);
        HandleRaycast(OVRInput.Button.PrimaryIndexTrigger, rightController, ref attachedB, ref anchorPointB);

        // Movement Logic
        ApplyGrappleLogic(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController),
                          OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController));

        // Thumbstick Movement
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, leftController);
        Vector3 moveDir = eyeTransform.rotation * new Vector3(stick.x, 0, stick.y);
        moveDir.y = 0;
        moveDir = moveDir.normalized * moveSpeed;
        rb.AddForce(moveDir, ForceMode.Acceleration);

        // Jump
        if (OVRInput.GetDown(OVRInput.Button.One, leftController) || OVRInput.GetDown(OVRInput.Button.One, rightController))
            ExecuteJump();

        ApplyVelocityConstraints();
    }

    void HandleRaycast(OVRInput.Button btn, OVRInput.Controller controller, ref bool isAttached, ref Vector3 anchorPoint)
    {
        if (OVRInput.GetDown(btn, controller))
        {
            if (!isAttached)
            {
                Vector3 worldOrigin = transform.TransformPoint(OVRInput.GetLocalControllerPosition(controller));
                Vector3 dir = (transform.rotation * OVRInput.GetLocalControllerRotation(controller)) * Vector3.forward;
                if (Physics.Raycast(worldOrigin, dir, out RaycastHit hit, maxDistance, combinedMask))
                {
                    anchorPoint = hit.point;
                    isAttached = true;
                    AttachGrapple((controller == leftController) ? 0 : 1, hit.point);
                }
            }
            else isAttached = false;
        }
    }

    void UpdateRayVisual(LineRenderer line, OVRInput.Controller controller)
    {
        if (line == null) return;

        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(controller);
        Vector3 worldOrigin = transform.TransformPoint(controllerPos);
        Vector3 dir = (transform.rotation * OVRInput.GetLocalControllerRotation(controller)) * Vector3.forward;

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