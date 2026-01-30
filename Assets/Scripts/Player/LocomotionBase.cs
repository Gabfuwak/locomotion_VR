using NUnit.Framework;
using Oculus.Interaction.Input;
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

    [Header("Visuals")]
    [SerializeField] protected LineRenderer leftLine;
    [SerializeField] protected LineRenderer rightLine;

    // Attach points
    protected Vector3 anchorPointA;
    protected Vector3 anchorPointB;

    // Game Mechanism Variables
    public ParkourCounter parkourCounter;
    public SelectionTaskMeasure selectionTaskMeasure;
    public GameObject hmd;
    public string Stage;

    // Player Locomotion Variables
    protected Rigidbody rb;
    protected bool attachedA = false;
    protected bool attachedB = false;
    protected bool isGrounded;
    private float currentLengthA;
    private float currentLengthB;

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

    protected void AttachGrapple(int grappleId, Vector3 hitPoint)
    {
        if (grappleId == 0)
        {
            currentLengthA = (hitPoint - transform.position).magnitude;
            leftLine.SetPosition(1, hitPoint);
        }
        if (grappleId == 1)
        {
            currentLengthB = (hitPoint - transform.position).magnitude;
            rightLine.SetPosition(1, hitPoint);
        }
    }

    protected void ApplyGrappleLogic(bool pullingA, bool pullingB)
    {
        Vector3 totalPullForce = Vector3.zero;

        // Handle Anchor A
        if (attachedA)
        {
            float distanceToA = Vector3.Distance(transform.position, anchorPointA);
            leftLine.SetPosition(0, rb.position);

            if (pullingA)
            {
                totalPullForce += (anchorPointA - transform.position).normalized * pullAcceleration;
                currentLengthA = distanceToA;
            }
            else
            {
                if (distanceToA > currentLengthA)
                {
                    ApplyDistanceConstraint(anchorPointA, currentLengthA);
                }
            }
        }

        if (attachedB)
        {
            float distanceToB = Vector3.Distance(transform.position, anchorPointB);
            rightLine.SetPosition(0, rb.position);

            if (pullingB)
            {
                totalPullForce += (anchorPointB - transform.position).normalized * pullAcceleration;
                currentLengthB = distanceToB;
            }
            else if (distanceToB > currentLengthB)
            {
                ApplyDistanceConstraint(anchorPointB, currentLengthB);
            }
        }

        if (totalPullForce != Vector3.zero)
            rb.AddForce(totalPullForce * Time.deltaTime, ForceMode.Acceleration);
    }

    private void ApplyDistanceConstraint(Vector3 anchorPos, float maxLength)
    {
        Vector3 directionToAnchor = (anchorPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, anchorPos);

        float extension = distance - maxLength;
        transform.position += directionToAnchor * extension;

        float dot = Vector3.Dot(rb.linearVelocity, directionToAnchor);
        if (dot < 0)
        {
            rb.linearVelocity -= dot * directionToAnchor;
        }
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

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
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
        }
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