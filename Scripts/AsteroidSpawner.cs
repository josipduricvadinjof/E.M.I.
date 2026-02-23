using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid asteroidPrefab;
    public float spawnDistance = 12f;
    public float initialSpawnRate = 1f;
    public float spawnRateDecrease = 0.01f; // How much to decrease the spawn rate per spawn
    public float minSpawnRate = 0.1f; // Minimum spawn rate to prevent it from reaching zero
    public int amountPerSpawn = 1;
    [Range(0f, 45f)]
    public float trajectoryVariance = 15f;

    private float currentSpawnRate;

    private void Start()
    {
        currentSpawnRate = initialSpawnRate;
        StartCoroutine(SpawnAsteroids());
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            Spawn();

            // Wait for the current spawn rate, then reduce it for next time
            yield return new WaitForSeconds(currentSpawnRate);
            currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate - spawnRateDecrease);
        }
    }

    public void Spawn()
    {
        for (int i = 0; i < amountPerSpawn; i++)
        {
            // Choose a random direction from the center of the spawner and
            // spawn the asteroid a distance away
            Vector3 spawnDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPoint = transform.position + (spawnDirection * spawnDistance);

            // Calculate a random variance in the asteroid's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            // Create the new asteroid by cloning the prefab and set a random
            // size within the range
            Asteroid asteroid = Instantiate(asteroidPrefab, spawnPoint, rotation);
            asteroid.size = Random.Range(asteroid.minSize, asteroid.maxSize);

            // Set the trajectory to move in the direction of the spawner
            Vector2 trajectory = rotation * -spawnDirection;
            asteroid.SetTrajectory(trajectory);
        }
    }
}
