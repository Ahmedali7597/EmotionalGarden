using UnityEngine;

/// <summary>
/// Spawns slimes in the GardenScene only when the selected emotion is Energetic.
/// Slimes wander around within the garden boundary using their Creature component.
/// </summary>
public class GardenSlimeSpawner : MonoBehaviour
{
    [Header("Slime Settings")]
    [Tooltip("Array of slime prefabs to randomly pick from when spawning.")]
    public GameObject[] slimePrefabs;

    [Tooltip("Number of slimes to spawn.")]
    public int spawnCount = 5;

    [Tooltip("Radius around the spawn center to scatter slimes.")]
    public float spawnRadius = 2.0f;

    [Header("Boundary")]
    [Tooltip("The boundary collider that defines the garden area.")]
    public Collider2D boundary;

    void Start()
    {
        // Only spawn slimes when the emotion is Energetic
        if (GameData.Instance != null && GameData.Instance.SelectedEmotion == GameData.Emotion.Energetic)
        {
            SpawnSlimes();
            Debug.Log($"[GardenSlimeSpawner] Emotion is Energetic — spawned {spawnCount} slimes.");
        }
        else
        {
            Debug.Log("[GardenSlimeSpawner] Emotion is not Energetic — no slimes spawned.");
        }
    }

    /// <summary>
    /// Spawns random slimes at random positions within the boundary.
    /// Each slime's Creature movement bounds are set to match the boundary.
    /// </summary>
    private void SpawnSlimes()
    {
        if (slimePrefabs == null || slimePrefabs.Length == 0)
        {
            Debug.LogWarning("[GardenSlimeSpawner] No slime prefabs assigned.");
            return;
        }

        // Calculate bounds from the boundary collider
        float minX, maxX, minY, maxY;
        if (boundary != null)
        {
            Bounds b = boundary.bounds;
            minX = b.min.x;
            maxX = b.max.x;
            minY = b.min.y;
            maxY = b.max.y;
        }
        else
        {
            // Fallback to camera bounds if no boundary assigned
            Camera cam = Camera.main;
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;
            Vector3 camPos = cam.transform.position;
            minX = camPos.x - camWidth + 1f;
            maxX = camPos.x + camWidth - 1f;
            minY = camPos.y - camHeight + 1f;
            maxY = camPos.y + camHeight - 1f;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            // Pick a random slime prefab
            int prefabIndex = Random.Range(0, slimePrefabs.Length);
            GameObject selectedPrefab = slimePrefabs[prefabIndex];

            // Random position within the boundary
            Vector3 spawnPos = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );

            GameObject slime = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

            // Configure the Creature component bounds so slimes stay within the garden
            Creature creature = slime.GetComponent<Creature>();
            if (creature != null)
            {
                creature.minX = minX;
                creature.maxX = maxX;
                creature.minY = minY;
                creature.maxY = maxY;
            }

            // Disable SlimeObstacle if present (it's for RunGame, not the garden)
            var obstacle = slime.GetComponent<SlimeObstacle>();
            if (obstacle != null)
            {
                obstacle.enabled = false;
            }
        }
    }
}
