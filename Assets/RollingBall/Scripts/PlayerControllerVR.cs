using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControllerVR : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Text textCount;
    [SerializeField] Text winText;
    [SerializeField] GameObject winObject;

    [Header("Movement Settings")]
    public float maxSpeed = 50f;
    public float accelerationSpeed = 5f;

    [Header("VR Settings")]
    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;

    private Rigidbody rb;
    int count;
    bool gameOver;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountUI();
        winObject.SetActive(false);
        gameOver = false;
    }

    void FixedUpdate()
    {
        /*#if (!gameOver)
        {*/
            // Get input from VR controller thumbstick (left controller)
            Vector2 movementVector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, leftController);

            OnMove(movementVector);
        /*}
        else
        {
            // Press A button on either controller to restart
            if (OVRInput.GetDown(OVRInput.Button.One, leftController) ||
                OVRInput.GetDown(OVRInput.Button.One, rightController))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }*/
    }

    private void OnMove(Vector2 movementVector)
    {
        Vector3 acceleration = new Vector3(movementVector.x, 0f, movementVector.y) * accelerationSpeed;

        rb.AddForce(acceleration);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity / rb.linearVelocity.magnitude * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameOver && other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountUI();
            if (count >= 10)
            {
                Destroy(GameObject.FindGameObjectWithTag("Enemy"));
                gameOver = true;
                winObject.SetActive(true);
                winText.text = "You Won!";
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameOver = true;
            gameObject.GetComponent<Renderer>().enabled = false;
            rb.linearVelocity = new Vector3(0f, 0f, 0f);
            winObject.SetActive(true);
            winText.text = "You Lose!";
        }
    }

    void SetCountUI()
    {
        textCount.text = "Count: " + count.ToString();
    }
}
