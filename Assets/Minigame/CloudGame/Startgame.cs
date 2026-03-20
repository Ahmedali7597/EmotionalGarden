using UnityEngine;

public class Startgame : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject sunPrefab;
    public int numberOfClouds = 10;
    
    // Padding keeps the clouds from spawning exactly on the very edge of the screen
    public float padding = 1.5f; 

    void Start()
    {
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
            
            Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);
            
            // Spawn the sun exactly where the first cloud is spawned
            if (i == 0)
            {
                Instantiate(sunPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}