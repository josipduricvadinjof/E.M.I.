using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance; // Singleton instance
    public AudioClip defaultBackgroundMusic; // Music for all scenes except Game
    public AudioClip gameSceneMusic; // Music for the Game scene
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes

            // Ensure there's an AudioSource component
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configure the AudioSource
            audioSource.loop = true;

            // Start playing the default background music
            if (defaultBackgroundMusic != null)
            {
                audioSource.clip = defaultBackgroundMusic;
                audioSource.Play();
            }

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game" && gameSceneMusic != null)
        {
            // Stop the default music and play the Game scene music
            audioSource.Stop();
            audioSource.clip = gameSceneMusic;
            audioSource.Play();
        }
        else if (scene.name != "Game" && audioSource.clip != defaultBackgroundMusic)
        {
            // Resume playing the default background music
            audioSource.Stop();
            audioSource.clip = defaultBackgroundMusic;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
