using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveMaxSpeed = 5f;
    [SerializeField] private float groundAcceleration = 5f;
    [SerializeField] private float skyAcceleration = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float damping = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private AnchorThrowing at;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        at = GetComponent<AnchorThrowing>();
    }

    private void Update()
    {
        // Check if the player is grounded using a sphere cast
        isGrounded = Physics.CheckSphere(
            transform.position,
            groundCheckDistance,
            groundLayer
        );

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");



        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        moveDirection = transform.TransformDirection(moveDirection);

        Vector3 moveCurrentAcc = groundAcceleration * Time.deltaTime * moveDirection;



        if (Input.GetKey(KeyCode.LeftShift) && at.Attached)
        {
            Vector3 anchorDir = (at.AnchorPos - rb.position).normalized;
            moveCurrentAcc += skyAcceleration * Time.deltaTime * anchorDir;
        }


        // Apply movement relative to the player's rotation
        rb.AddForce(moveCurrentAcc);

        float currentVelocity = rb.linearVelocity.magnitude;

        if (currentVelocity > moveMaxSpeed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, moveMaxSpeed);
        }

        if (isGrounded) rb.linearVelocity *= Mathf.Pow(damping, Time.deltaTime);

        if (at.Attached) Debug.DrawRay(rb.position, (at.AnchorPos - rb.position), Color.red);
    }
}


