using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Slime Settings")]
    // Array to hold the 10 different slime prefabs.
    public GameObject[] slimePrefabs; 
    
    [Header("Difficulty Settings (Spawn Rate)")]
    public float spawnRate = 3.0f;       // Starting spawn interval (3 seconds = very slow)
    public float minSpawnRate = 0.5f;    // The fastest possible spawn interval (0.5 seconds)
    public float decreaseRate = 0.05f;   // How much time is reduced per spawn (Difficulty spike)
    private float timer = 0f;

    [Header("Game Timer Settings")]
    public float targetSurviveTime = 60.0f; // Goal time to survive (60 seconds)
    private float currentSurviveTime = 0f;  // Time survived so far
    private bool isGameOver = false;        // To check if the game has ended

    void Update()
    {
        // If the game is already over (survived 60 seconds), stop running this code.
        if (isGameOver) return;

        // 1. Keep track of the total time survived.
        currentSurviveTime += Time.deltaTime;

        // 2. If the player survived for 60 seconds? Victory!
        if (currentSurviveTime >= targetSurviveTime)
        {
            GameClear();
            return; // Stop spawning and exit the function.
        }

        // 3. Run the slime spawn timer.
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnSlime(); // Spawn a slime!
            timer = 0f;   // Reset the spawn timer

            // ★ Difficulty Increase Logic: Decrease the spawn interval each time a slime appears.
            if (spawnRate > minSpawnRate)
            {
                spawnRate -= decreaseRate;
            }
        }
    }

    void SpawnSlime()
    {
        // Safety check: Prevent errors if the array is empty.
        if (slimePrefabs.Length == 0) return;

        // Pick a random number between 0 and the number of prefabs.
        int randomIndex = Random.Range(0, slimePrefabs.Length);
        
        // Select the slime prefab corresponding to the random number.
        GameObject selectedSlime = slimePrefabs[randomIndex];

        // Create the selected slime on the screen at the Spawner's position.
        Instantiate(selectedSlime, transform.position, Quaternion.identity);
    }

    // Function called when successfully surviving for 60 seconds.
    void GameClear()
    {
        isGameOver = true;
        Debug.Log("Survived for 60 seconds! Game Clear!");

        // Show EndGame screen (Success - survived!)
        EndGameUI.Show(true);
    }
}