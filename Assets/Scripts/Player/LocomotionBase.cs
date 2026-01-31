using UnityEngine;

public abstract class LocomotionBase : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] protected LayerMask combinedMask;
    [SerializeField] protected float maxDistance = 200f;
    [SerializeField] protected float damping = 0.1f;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 30f;
    [SerializeField] protected float jumpForce = 3f;
    [SerializeField] protected float flyForce = 3f;
    [SerializeField] protected float pullAcceleration = 5000f;
    [SerializeField] protected float moveMaxSpeed = 25f;
    [SerializeField] protected float gasComsumption = 0.3f;
    [SerializeField] protected float gasRefuel = 0.15f;

    [Header("Visuals")]
    [SerializeField] protected LineRenderer leftLine;
    [SerializeField] protected LineRenderer rightLine;

    protected Vector3 anchorPointA, anchorPointB;
    protected Rigidbody rb;
    protected bool attachedA, attachedB;
    protected bool isGrounded, isWalled;
    private float currentLengthA, currentLengthB;
    public float CurrentGas = 1.0f;

    // References for Game Mechanisms
    public ParkourCounter parkourCounter;
    public SelectionTaskMeasure selectionTaskMeasure;
    public GameObject hmd;
    public string Stage;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Coins
        Coin[] coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);
        foreach (var c in coins) c.Player = this;

        // Banners
        Banner[] banners = FindObjectsByType<Banner>(FindObjectsSortMode.None);
        foreach (var b in banners) b.Player = this;

        // Object Interaction Task
        ObjectInteractionTask[] oits = FindObjectsByType<ObjectInteractionTask>(FindObjectsSortMode.None);
        foreach (var o in oits) o.Player = this;
    }

    protected virtual void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + 0.6f * Vector3.down, 0.3f, combinedMask);
        isWalled = Physics.CheckSphere(transform.position, 1.0f, combinedMask);
        CurrentGas = Mathf.Clamp(CurrentGas, 0.0f, 1.0f);
    }

    // Logic for shooting or detaching a grapple
    protected void HandleGrappleToggle(int id, Vector3 origin, Vector3 direction)
    {
        ref bool attached = ref (id == 0 ? ref attachedA : ref attachedB);
        ref Vector3 anchor = ref (id == 0 ? ref anchorPointA : ref anchorPointB);
        LineRenderer line = (id == 0 ? leftLine : rightLine);

        if (!attached)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, combinedMask))
            {
                anchor = hit.point;
                attached = true;
                if (id == 0) currentLengthA = (hit.point - transform.position).magnitude;
                else currentLengthB = (hit.point - transform.position).magnitude;
                line.SetPosition(1, hit.point);
            }
        }
        else attached = false;
    }

    // Visual helper for "aiming" rays
    protected void UpdateRayVisual(LineRenderer line, Vector3 origin, Vector3 direction)
    {
        if (line == null) return;
        line.SetPosition(0, origin);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, combinedMask))
            line.SetPosition(1, hit.point);
        else
            line.SetPosition(1, origin + (direction * maxDistance));
    }

    protected void ApplyGrappleLogic(bool pullingA, bool pullingB)
    {
        Vector3 totalPullForce = Vector3.zero;
        ProcessAnchor(0, pullingA, ref attachedA, ref anchorPointA, ref currentLengthA, leftLine, ref totalPullForce);
        ProcessAnchor(1, pullingB, ref attachedB, ref anchorPointB, ref currentLengthB, rightLine, ref totalPullForce);

        if (totalPullForce != Vector3.zero)
            rb.AddForce(totalPullForce * Time.deltaTime, ForceMode.Acceleration);
    }

    private void ProcessAnchor(int id, bool pulling, ref bool attached, ref Vector3 anchor, ref float curLen, LineRenderer line, ref Vector3 totalForce)
    {
        if (!attached) return;
        float dist = Vector3.Distance(transform.position, anchor);
        line.SetPosition(0, rb.position);

        if (pulling)
        {
            totalForce += (anchor - transform.position).normalized * pullAcceleration;
            curLen = dist;
        }
        else if (dist > curLen)
        {
            ApplyDistanceConstraint(anchor, curLen);
        }
    }

    private void ApplyDistanceConstraint(Vector3 anchorPos, float maxLength)
    {
        Vector3 dir = (anchorPos - transform.position).normalized;
        float extension = Vector3.Distance(transform.position, anchorPos) - maxLength;
        transform.position += dir * extension;

        float dot = Vector3.Dot(rb.linearVelocity, dir);
        if (dot < 0) rb.linearVelocity -= dot * dir;
    }

    protected void ExecuteJump() 
    { 
        if (isGrounded || (isWalled && (attachedA || attachedB))) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
    }
    protected void ExecuteFlying()
    {
        if (CurrentGas > 0.0f)
        {
            rb.AddForce(Vector3.up * flyForce, ForceMode.Acceleration);
            CurrentGas -= gasComsumption * Time.deltaTime;
        }
    }

    protected void ApplyVelocityConstraints()
    {
        if (rb.linearVelocity.magnitude > moveMaxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * moveMaxSpeed;

        if (isGrounded)
            rb.linearVelocity *= Mathf.Pow(damping, Time.deltaTime);
    }

    public void CollideWithBanner(GameObject banner)
    {
        Stage = banner.name;
        parkourCounter.isStageChange = true;
    }

    public void CollideWithOIT(GameObject objectInteractionTask)
    {
        selectionTaskMeasure.isTaskStart = true;
        selectionTaskMeasure.scoreText.text = "";
        selectionTaskMeasure.partSumErr = 0f;
        selectionTaskMeasure.partSumTime = 0f;
        // rotation: facing the user's entering direction
        float tempValueY = objectInteractionTask.transform.position.y > 0 ? 12 : 0;
        Vector3 tmpTarget = new(hmd.transform.position.x, tempValueY, hmd.transform.position.z);
        selectionTaskMeasure.taskUI.transform.LookAt(tmpTarget);
        selectionTaskMeasure.taskUI.transform.Rotate(new Vector3(0, 180f, 0));
        selectionTaskMeasure.taskStartPanel.SetActive(true);
    }

    public void CollideWithCoin(GameObject coin)
    {
        parkourCounter.coinCount += 1;
        GetComponent<AudioSource>().Play();
        coin.SetActive(false);

    }
}