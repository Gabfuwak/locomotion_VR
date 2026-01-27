using OVR.OpenVR;
using UnityEngine;

public class LocomotionTechnique : MonoBehaviour
{
    public Transform leftControllerAnchor;
    public Transform rightControllerAnchor;

    [Header("Anchor Visuals")]
    [SerializeField] GameObject anchorPointA;
    [SerializeField] GameObject anchorPointB;

    [Header("Physics Settings")]
    [SerializeField] LayerMask combinedMask;
    [SerializeField] float maxDistance = 200f;
    [SerializeField] private float moveMaxSpeed = 25f;
    [SerializeField] private float pullAcceleration = 5000f;
    [SerializeField] private float damping = 0.1f;

    private Rigidbody rb;
    private bool attachedA = false;
    private bool attachedB = false;

    // Game Mechanism Variables
    public ParkourCounter parkourCounter;
    public SelectionTaskMeasure selectionTaskMeasure;
    public GameObject hmd;
    public string stage;

    private OVRInput.Controller leftController;
    private OVRInput.Controller rightController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        leftController = OVRInput.Controller.LTouch;
        rightController = OVRInput.Controller.RTouch;
    }

    void Update()
    {
        // HANDLE LEFT HAND (Anchor A)
        HandleAnchor(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Button.PrimaryHandTrigger, ref attachedA, anchorPointA, leftController);

        // HANDLE RIGHT HAND (Anchor B)
        HandleAnchor(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Button.PrimaryHandTrigger, ref attachedB, anchorPointB, rightController);

        MovePlayer();

        // Respawn Logic
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart) transform.position = parkourCounter.currentRespawnPos;
        }
    }

    // Generic function to handle raycasting for either hand
    void HandleAnchor(OVRInput.Button shootBtn, OVRInput.Button releaseBtn, ref bool isAttached, GameObject visualPoint, OVRInput.Controller controllerType)
    {
        // 2. Shoot Ray
        if (!isAttached && OVRInput.GetDown(shootBtn, controllerType))
        {
            // Define the origin and direction clearly
            Vector3 rayOrigin = rb.position;
            Quaternion orientation = OVRInput.GetLocalControllerRotation(controllerType);
            Vector3 rayDirection = orientation * Vector3.forward;

            // Visual debug tool (only visible in Scene view during Play mode)
            Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.cyan, 1.0f);

            Ray ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, combinedMask))
            {
                visualPoint.transform.position = hit.point;
                visualPoint.SetActive(true);
                isAttached = true;

                // Haptic feedback for a successful hit
                OVRInput.SetControllerVibration(0.1f, 0.1f, controllerType);
            } else
            {
                visualPoint.transform.position = rayOrigin + rayDirection * maxDistance;
                visualPoint.SetActive(true);
                isAttached = true;

                // Haptic feedback for a successful hit
                OVRInput.SetControllerVibration(0.1f, 0.1f, controllerType);
            }
        }
        // 3. Release
        else if (isAttached && OVRInput.GetDown(releaseBtn, controllerType))
        {
            visualPoint.SetActive(false);
            isAttached = false;
        }
    }

    void MovePlayer()
    {
        Vector3 totalPullForce = Vector3.zero;

        // Pull toward A
        if (attachedA)
            totalPullForce += (anchorPointA.transform.position - transform.position).normalized * pullAcceleration;

        // Pull toward B
        if (attachedB)
            totalPullForce += (anchorPointB.transform.position - transform.position).normalized * pullAcceleration;

        // Apply Forces
        if (totalPullForce != Vector3.zero)
        {
            rb.AddForce(totalPullForce * Time.deltaTime, ForceMode.Acceleration);
        }

        ApplyVelocityLogic();
    }

    void ApplyVelocityLogic()
    {
        // Cap Speed
        if (rb.linearVelocity.magnitude > moveMaxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * moveMaxSpeed;

        // Friction/Damping
        rb.linearVelocity *= Mathf.Pow(damping, Time.deltaTime);
    }


    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("objectInteractionTask"))
        {
            selectionTaskMeasure.isTaskStart = true;
            selectionTaskMeasure.scoreText.text = "";
            selectionTaskMeasure.partSumErr = 0f;
            selectionTaskMeasure.partSumTime = 0f;
            // rotation: facing the user's entering direction
            float tempValueY = other.transform.position.y > 0 ? 12 : 0;
            Vector3 tmpTarget = new(hmd.transform.position.x, tempValueY, hmd.transform.position.z);
            selectionTaskMeasure.taskUI.transform.LookAt(tmpTarget);
            selectionTaskMeasure.taskUI.transform.Rotate(new Vector3(0, 180f, 0));
            selectionTaskMeasure.taskStartPanel.SetActive(true);
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // These are for the game mechanism.
    }
}