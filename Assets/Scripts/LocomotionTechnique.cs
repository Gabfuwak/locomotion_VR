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
    [SerializeField] private float damping = 0.1f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float flyForce = 3f;
    [SerializeField] private float pullAcceleration = 5000f;
    [SerializeField] private float moveMaxSpeed = 25f;

    [Header("Eye Transform")]
    [SerializeField] Transform eyeTransform;

    private Rigidbody rb;
    private bool attachedA = false;
    private bool attachedB = false;

    // Movement
    bool isGrounded;

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
        HandleAnchor(OVRInput.Button.PrimaryIndexTrigger, ref attachedA, anchorPointA, leftController);

        // HANDLE RIGHT HAND (Anchor B)
        HandleAnchor(OVRInput.Button.PrimaryIndexTrigger, ref attachedB, anchorPointB, rightController);

        MovePlayer();

        // Respawn Logic
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter && parkourCounter.parkourStart) transform.position = parkourCounter.currentRespawnPos;
        }
        
        ApplyVelocityLogic();
    }

    // Generic function to handle raycasting for either hand
    void HandleAnchor(OVRInput.Button shootBtn, ref bool isAttached, GameObject visualPoint, OVRInput.Controller controller)
    {
        // 2. Shoot Ray
        if (OVRInput.GetDown(shootBtn, controller))
        {
            if (!isAttached)
            {
                Vector3 rayOrigin = rb.position;
                Quaternion controllerOrientation = OVRInput.GetLocalControllerRotation(controller);
                Quaternion playerRotation = transform.rotation;

                Quaternion combinedRotation = playerRotation * controllerOrientation;
                Vector3 rayDirection = combinedRotation * Vector3.forward;

                Ray ray = new Ray(rayOrigin, rayDirection);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, combinedMask))
                {
                    visualPoint.transform.position = hit.point;
                    visualPoint.SetActive(true);
                    isAttached = true;
                }
            }
            else
            {
                visualPoint.SetActive(false);
                isAttached = false;
            }

        }
        
    }

    void MovePlayer()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, combinedMask);
        Vector3 totalPullForce = Vector3.zero;

        if (attachedA && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController))
            totalPullForce += (anchorPointA.transform.position - transform.position).normalized * pullAcceleration;

        if (attachedB && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController))
            totalPullForce += (anchorPointB.transform.position - transform.position).normalized * pullAcceleration;

        if (totalPullForce != Vector3.zero)
        {
            rb.AddForce(totalPullForce * Time.deltaTime, ForceMode.Acceleration);
        }

        Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, leftController);
        Vector3 moveDirection = new Vector3(leftThumbstick.x, 0f, leftThumbstick.y) * moveSpeed;
        moveDirection = eyeTransform.rotation * moveDirection;
        rb.AddForce(moveDirection, ForceMode.Acceleration);


        if (OVRInput.GetDown(OVRInput.Button.One, leftController) || OVRInput.GetDown(OVRInput.Button.One, rightController))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
            }
        }
    }

    void ApplyVelocityLogic()
    {
        if (rb.linearVelocity.magnitude > moveMaxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * moveMaxSpeed;

        if (isGrounded) rb.linearVelocity *= Mathf.Pow(damping, Time.deltaTime);
        
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