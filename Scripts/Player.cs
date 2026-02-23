using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private Bullet bulletPrefab;

    public float thrustSpeed = 1f;
    public bool thrusting { get; private set; }

    public float rotationSpeed = 0.1f;
    private float turnDirection;

    public float respawnDelay = 3f;
    public float respawnInvulnerability = 3f;

    public bool screenWrapping = true;
    private Bounds screenBounds;

    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");

        // Disable all boundaries if screen wrapping is enabled
        for (int i = 0; i < boundaries.Length; i++)
        {
            boundaries[i].SetActive(!screenWrapping);
        }

        // Convert screen space bounds to world space bounds
        screenBounds = new Bounds();
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));
    }

    private void OnEnable()
    {
        // Turn off collisions for a few seconds after spawning to ensure the
        // player has enough time to safely move away from asteroids
        TurnOffCollisions();
        Invoke(nameof(TurnOnCollisions), respawnInvulnerability);
    }

    private void Update()
    {
        if (moveActionToUse != null && moveActionToUse.action != null)
        {
            Vector2 moveDirection = moveActionToUse.action.ReadValue<Vector2>();
            thrusting = moveDirection.magnitude > 0.1f; // If the joystick is being moved

            // Move the player based on joystick input
            rb.velocity = moveDirection * speed;

            // Rotate player 90 degrees to the left from its current position
            if (moveDirection.magnitude > 0.1f)  // Only rotate if moving
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            Debug.LogWarning("MoveActionToUse or its action is not set in the Inspector!");
        }
    }

    private void FixedUpdate()
    {
        if (thrusting)
        {
            rb.AddForce(transform.up * thrustSpeed);
        }

        if (screenWrapping)
        {
            ScreenWrap();
        }
    }

    private void ScreenWrap()
    {
        if (rb.position.x > screenBounds.max.x + 0.5f)
        {
            rb.position = new Vector2(screenBounds.min.x - 0.5f, rb.position.y);
        }
        else if (rb.position.x < screenBounds.min.x - 0.5f)
        {
            rb.position = new Vector2(screenBounds.max.x + 0.5f, rb.position.y);
        }
        else if (rb.position.y > screenBounds.max.y + 0.5f)
        {
            rb.position = new Vector2(rb.position.x, screenBounds.min.y - 0.5f);
        }
        else if (rb.position.y < screenBounds.min.y - 0.5f)
        {
            rb.position = new Vector2(rb.position.x, screenBounds.max.y + 0.5f);
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.Shoot(transform.up);
    }

    public void OnShootButtonPressed()
    {
        Shoot();
    }

    private void TurnOffCollisions()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
    }

    private void TurnOnCollisions()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            GameManager.Instance.OnPlayerDeath(this);
        }
    }
}
