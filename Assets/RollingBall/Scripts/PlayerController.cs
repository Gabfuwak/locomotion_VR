using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RollingBallPlayerController : MonoBehaviour
{
    [SerializeField] Text textCount;
    [SerializeField] Text winText;
    [SerializeField] GameObject winObject;
    public float maxSpeed = 50f;
    public float accelerationSpeed = 5f;
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
        if (!gameOver)
        {
            Vector2 movementVector;
            movementVector.x = Input.GetAxis("Horizontal");
            movementVector.y = Input.GetAxis("Vertical");

            OnMove(movementVector);
        } else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
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
            gameObject.GetComponent<Renderer>().enabled = false; // Do not destroy it but hide it
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
