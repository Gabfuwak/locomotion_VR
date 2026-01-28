using UnityEngine;

public class LocomotionVR : LocomotionBase
{
    [SerializeField] private Transform eyeTransform;
    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;

    void Update()
    {
        // Handle Shooting Grapples
        HandleRaycast(OVRInput.Button.PrimaryIndexTrigger, leftController, ref attachedA, anchorPointA);
        HandleRaycast(OVRInput.Button.PrimaryIndexTrigger, rightController, ref attachedB, anchorPointB);

        // Movement Logic
        ApplyGrappleLogic(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController),
                          OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController));

        // Thumbstick Movement
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, leftController);
        Vector3 moveDir = eyeTransform.rotation * new Vector3(stick.x, 0, stick.y) * moveSpeed;
        rb.AddForce(moveDir, ForceMode.Acceleration);

        // Jump
        if (OVRInput.GetDown(OVRInput.Button.One, leftController) || OVRInput.GetDown(OVRInput.Button.One, rightController))
            ExecuteJump();

        ApplyVelocityConstraints();
    }

    void HandleRaycast(OVRInput.Button btn, OVRInput.Controller controller, ref bool isAttached, GameObject visual)
    {
        if (OVRInput.GetDown(btn, controller))
        {
            if (!isAttached)
            {
                Vector3 dir = (transform.rotation * OVRInput.GetLocalControllerRotation(controller)) * Vector3.forward;
                if (Physics.Raycast(rb.position, dir, out RaycastHit hit, maxDistance, combinedMask))
                {
                    visual.transform.position = hit.point;
                    visual.SetActive(true);
                    isAttached = true;
                }
            }
            else { visual.SetActive(false); isAttached = false; }
        }
    }
}