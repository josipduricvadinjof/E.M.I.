using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text bestscoreText;
    [SerializeField] private AudioSource gameOverSound;


    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();

        bestscoreText.text = PlayerPrefs.GetInt("EMI_Best_Score", 0).ToString();

    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }

        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        Respawn();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();


        if (score >= PlayerPrefs.GetInt("EMI_Best_Score", 0))
        {
            PlayerPrefs.SetInt("EMI_Best_Score", score);
            PlayerPrefs.Save();

            bestscoreText.text = PlayerPrefs.GetInt("EMI_Best_Score", 0).ToString();
        }

    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }

    private void Respawn()
    {
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);
    }

    public void OnAsteroidDestroyed(Asteroid asteroid)
    {
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.Play();

        if (asteroid.size < 0.7f)
        {
            SetScore(score + 100); // small asteroid
        }
        else if (asteroid.size < 1.4f)
        {
            SetScore(score + 50); // medium asteroid
        }
        else
        {
            SetScore(score + 25); // large asteroid
        }
    }

    public void OnPlayerDeath(Player player)
    {
        player.gameObject.SetActive(false);

        explosionEffect.transform.position = player.transform.position;
        explosionEffect.Play();

        SetLives(lives - 1);

        if (lives <= 0)
        {
            gameOverUI.SetActive(true);
        }
        else
        {
            Invoke(nameof(Respawn), player.respawnDelay);
        }

        gameOverSound.Play();
    }

}