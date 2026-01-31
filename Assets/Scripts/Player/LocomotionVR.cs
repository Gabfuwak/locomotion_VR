using UnityEngine;

public class LocomotionVR : LocomotionBase
{
    [SerializeField] private Transform eyeTransform;
    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;

    protected override void Update()
    {
        base.Update();

        // 1. Ray Visuals
        if (!attachedA) UpdateRayVisual(leftLine, GetCtrlWorldPos(leftController), GetCtrlWorldDir(leftController));
        if (!attachedB) UpdateRayVisual(rightLine, GetCtrlWorldPos(rightController), GetCtrlWorldDir(rightController));

        // 2. Grapple Inputs
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, leftController))
            HandleGrappleToggle(0, GetCtrlWorldPos(leftController), GetCtrlWorldDir(leftController));

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightController))
            HandleGrappleToggle(1, GetCtrlWorldPos(rightController), GetCtrlWorldDir(rightController));

        // 3. Movement Logic
        ApplyGrappleLogic(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController),
                          OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController));

        // 4. Thumbstick & Movement
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, leftController);
        Vector3 moveDir = Vector3.ProjectOnPlane(eyeTransform.rotation * new Vector3(stick.x, 0, stick.y), Vector3.up).normalized;
        rb.AddForce(moveDir * moveSpeed, ForceMode.Acceleration);

        // 5. Jump/Fly
        if (OVRInput.GetDown(OVRInput.Button.One, leftController) || OVRInput.GetDown(OVRInput.Button.One, rightController)) ExecuteJump();
        if (OVRInput.Get(OVRInput.Button.One, leftController) || OVRInput.Get(OVRInput.Button.One, rightController)) ExecuteFlying();
        else CurrentGas += gasRefuel * Time.deltaTime;

        ApplyVelocityConstraints();
    }

    private Vector3 GetCtrlWorldPos(OVRInput.Controller c) => transform.TransformPoint(OVRInput.GetLocalControllerPosition(c));
    private Vector3 GetCtrlWorldDir(OVRInput.Controller c) => (transform.rotation * OVRInput.GetLocalControllerRotation(c)) * Vector3.forward;
}