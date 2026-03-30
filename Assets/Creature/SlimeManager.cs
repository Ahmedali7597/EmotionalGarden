using UnityEngine;

public class SlimeManager : MonoBehaviour
{
    [Header("Slime Settings")]
    // Array to hold the slime prefabs.
    public GameObject[] slimePrefabs;

    // The number of slimes to spawn (modifiable in the Inspector).
    public int spawnCount = 5;

    // The radius to scatter the slimes so they don't overlap.
    public float spawnRadius = 2.0f;

    void Start()
    {
        // Spawn the specified number of random slimes when the game starts.
        SpawnRandomSlimes();
    }

    public void SpawnRandomSlimes()
    {
        // Safety check: Prevent errors if no slime prefabs are assigned.
        if (slimePrefabs.Length == 0)
        {
            Debug.LogWarning("No slime prefabs have been assigned!");
            return;
        }

        // Repeat the spawning process for the 'spawnCount' amount.
        for (int i = 0; i < spawnCount; i++)
        {
            // 1. Pick a random number between 0 and the number of prefabs.
            int randomIndex = Random.Range(0, slimePrefabs.Length);

            // 2. Select the slime prefab corresponding to the random number.
            GameObject selectedSlime = slimePrefabs[randomIndex];

            // 3. Create a random position scattered around the manager's current location.
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // 4. Actually instantiate (create) the slime on the screen!
            Instantiate(selectedSlime, spawnPosition, Quaternion.identity);
        }
    }
}