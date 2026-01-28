using UnityEngine;

public abstract class LocomotionBase : MonoBehaviour
{
    [Header("Anchor Visuals")]
    public GameObject anchorPointA;
    public GameObject anchorPointB;

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

    protected Rigidbody rb;
    protected bool attachedA = false;
    protected bool attachedB = false;
    protected bool isGrounded;

    // External Game Logic
    public ParkourCounter parkourCounter;
    public SelectionTaskMeasure selectionTaskMeasure;
    public GameObject hmdOrCamera;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void ApplyGrappleLogic(bool pullingA, bool pullingB)
    {
        Vector3 totalPullForce = Vector3.zero;

        if (attachedA && pullingA)
            totalPullForce += (anchorPointA.transform.position - transform.position).normalized * pullAcceleration;

        if (attachedB && pullingB)
            totalPullForce += (anchorPointB.transform.position - transform.position).normalized * pullAcceleration;
        Debug.Log(totalPullForce);
        if (totalPullForce != Vector3.zero)
            rb.AddForce(totalPullForce * Time.deltaTime, ForceMode.Acceleration);
    }

    protected void ExecuteJump()
    {
        if (isGrounded) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        else rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
    }

    protected void ApplyVelocityConstraints()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, combinedMask);

        if (rb.linearVelocity.magnitude > moveMaxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * moveMaxSpeed;

        if (isGrounded)
            rb.linearVelocity *= Mathf.Pow(damping, Time.deltaTime);
    }

    // Shared Trigger Logic
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("banner"))
        {
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount++;
            GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // ... include your selectionTaskMeasure logic here ...
    }
}