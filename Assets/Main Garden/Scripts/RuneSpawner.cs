using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns a random Rune prefab at a random position within the camera view every 30 seconds.
/// Only one rune is active at a time. If the player clicks the rune, a random minigame
/// is launched via MiniGameLauncher.
/// </summary>
public class RuneSpawner : MonoBehaviour
{
    [Header("Rune Settings")]
    [Tooltip("Array of Rune prefabs to randomly pick from when spawning.")]
    public GameObject[] runePrefabs;

    [Tooltip("Time in seconds between each rune spawn.")]
    public float spawnInterval = 15f;

    [Header("Spawn Area (camera-based)")]
    [Tooltip("Padding from the camera edges so the rune doesn't spawn off-screen.")]
    public float edgePadding = 1f;

    private GameObject currentRune;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Coroutine that waits spawnInterval seconds, then spawns a rune.
    /// Repeats forever.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRune();
        }
    }

    /// <summary>
    /// Spawns a random rune at a random position within the camera view.
    /// Destroys any existing rune first so only one is active at a time.
    /// </summary>
    private void SpawnRune()
    {
        if (runePrefabs == null || runePrefabs.Length == 0)
        {
            Debug.LogWarning("[RuneSpawner] No rune prefabs assigned.");
            return;
        }

        // Destroy previous rune if it still exists
        if (currentRune != null)
        {
            Destroy(currentRune);
        }

        // Calculate spawn bounds from the orthographic camera
        if (mainCam == null)
            mainCam = Camera.main;

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camPos = mainCam.transform.position;

        float minX = camPos.x - camWidth + edgePadding;
        float maxX = camPos.x + camWidth - edgePadding;
        float minY = camPos.y - camHeight + edgePadding;
        float maxY = camPos.y + camHeight - edgePadding;

        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        // Pick a random rune prefab
        int index = Random.Range(0, runePrefabs.Length);
        currentRune = Instantiate(runePrefabs[index], spawnPos, Quaternion.identity);

        // Disable puzzle-game-specific components so the rune just sits as a clickable object
        var dragObj = currentRune.GetComponent<DragObject>();
        if (dragObj != null)
            dragObj.enabled = false;

        var socket = currentRune.GetComponent<Socket>();
        if (socket != null)
            socket.enabled = false;

        var rb = currentRune.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Static;

        // Ensure the rune has a collider for click detection via raycast
        if (currentRune.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = currentRune.AddComponent<BoxCollider2D>();
            SpriteRenderer sr = currentRune.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                col.size = sr.sprite.bounds.size;
            }
        }

        // Set sorting order above plants (plants are 2, avatar is 10, runes are 5)
        SpriteRenderer runeSR = currentRune.GetComponent<SpriteRenderer>();
        if (runeSR != null)
        {
            runeSR.sortingOrder = 5;
        }

        // Add the clickable component so PlantClickHandler can detect it
        if (currentRune.GetComponent<RuneClickable>() == null)
        {
            currentRune.AddComponent<RuneClickable>();
        }

        Debug.Log($"[RuneSpawner] Rune '{runePrefabs[index].name}' spawned at {spawnPos}");
    }
}
