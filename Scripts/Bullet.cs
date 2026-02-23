using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed = 500f;
    public float maxLifetime = 10f;

    [Header("Audio Settings")]
    public AudioClip shootSound; // Drag and drop the sound effect here in the Inspector
    public AudioSource audioSource; // Assign an AudioSource from the Inspector or create one dynamically

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure an AudioSource is attached
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void Shoot(Vector2 direction)
    {
        // Add force to the bullet
        rb.AddForce(direction * speed);

        // Play the shooting sound effect
        PlayShootSound();

        // Destroy the bullet after it reaches its max lifetime
        Destroy(gameObject, maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet as soon as it collides with anything
        Destroy(gameObject);
    }

    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.clip = shootSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Shoot sound or AudioSource is missing!");
        }
    }
}
