using UnityEngine;

public class Startgame : MonoBehaviour
{
    // Changed from a single GameObject to an array (GameObject[])
    // This allows you to set the size in the Inspector and drop multiple prefabs.
    public GameObject[] cloudPrefabs; 
    
    public GameObject sunPrefab;
    public int numberOfClouds = 10;
    
    // Padding keeps the clouds from spawning exactly on the very edge of the screen
    public float padding = 1.5f; 

    void Start()
    {
        // Prevent errors if you forget to add prefabs in the Inspector
        if (cloudPrefabs.Length == 0)
        {
            Debug.LogError("Please assign at least one cloud prefab in the Inspector!");
            return;
        }

        // 1. Get the main camera
        Camera cam = Camera.main;
        
        // 2. Find the exact corners of the screen in 2D world space
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        // 3. Define the safe spawning range by adding our padding
        float minX = bottomLeft.x + padding;
        float maxX = topRight.x - padding;
        float minY = bottomLeft.y + padding;
        float maxY = topRight.y - padding;

        // Spawn clouds at random positions within the dynamic safe range
        for (int i = 0; i < numberOfClouds; i++)
        {
            float randomX = Random.Range(minX, maxX); 
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);
            
            // 4. Pick a random cloud prefab from the array
            int randomCloudIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject selectedCloudPrefab = cloudPrefabs[randomCloudIndex];

            // Spawn the randomly selected cloud
            Instantiate(selectedCloudPrefab, spawnPosition, Quaternion.identity);
            
            // Spawn the sun exactly where the first cloud is spawned
            if (i == 0)
            {
                Instantiate(sunPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}