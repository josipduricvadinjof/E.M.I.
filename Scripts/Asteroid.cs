using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] sprites;

    [Header("Asteroid Properties")]
    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;
    public float maxLifetime = 30f;

    [Header("Audio Settings")]
    public AudioClip destroySound; // Sound to play when destroyed
    public AudioSource audioSource; // AudioSource to play the sound

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure an AudioSource is attached
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // Set to 2D sound
            audioSource.volume = 1f;      // Full volume
        }
    }

    private void Start()
    {
        // Assign random properties to make each asteroid feel unique
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);

        // Set the scale and mass of the asteroid based on the assigned size
        transform.localScale = Vector3.one * size;
        rb.mass = size;

        // Destroy the asteroid after it reaches its max lifetime
        Destroy(gameObject, maxLifetime);
    }

    public void SetTrajectory(Vector2 direction)
    {
        // The asteroid only needs a force to be added once
        rb.AddForce(direction * movementSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Check if the asteroid is large enough to split in half
            if ((size * 0.5f) >= minSize)
            {
                CreateSplit();
                CreateSplit();
            }

            // Notify GameManager
            GameManager.Instance.OnAsteroidDestroyed(this);

            // Play the destroy sound on a separate object
            PlayDestroySound();

            // Destroy the asteroid immediately
            Destroy(gameObject);
        }
    }

    private Asteroid CreateSplit()
    {
        // Set the new asteroid position with a slight offset
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        // Create the new asteroid at half the size of the current one
        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;

        // Set a random trajectory
        half.SetTrajectory(Random.insideUnitCircle.normalized);

        return half;
    }

    private void PlayDestroySound()
    {
        if (destroySound != null)
        {
            // Create a temporary GameObject to play the sound
            GameObject soundObject = new GameObject("AsteroidDestroySound");
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();

            // Configure the AudioSource
            tempAudioSource.clip = destroySound;
            tempAudioSource.spatialBlend = 0f; // Set to 2D sound
            tempAudioSource.volume = 1f;      // Set volume
            tempAudioSource.Play();

            // Destroy the sound object after the clip finishes playing
            Destroy(soundObject, destroySound.length);
        }
        else
        {
            Debug.LogWarning("Destroy sound is missing!");
        }
    }
}
