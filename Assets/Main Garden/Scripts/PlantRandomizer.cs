using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Randomizes the positions of all plants in the garden at the start of each play session.
/// Each plant is placed on a unique dirt tile (ground_grass_bricks_264) so that
/// there is exactly one plant per dirt tile. The 9 plants are shuffled across the 9 tiles.
/// </summary>
public class PlantRandomizer : MonoBehaviour
{
    [Tooltip("The tilemap containing the dirt tiles.")]
    public Tilemap tilemap;

    [Tooltip("The name of the dirt tile to place plants on.")]
    public string dirtTileName = "ground_grass_bricks_264";

    void Start()
    {
        RandomizePlantPositions();
    }

    /// <summary>
    /// Finds all dirt tiles in the tilemap, collects all Plant objects,
    /// then shuffles and assigns each plant to a unique dirt tile center.
    /// </summary>
    private void RandomizePlantPositions()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("[PlantRandomizer] No tilemap assigned.");
            return;
        }

        // Collect all dirt tile world positions
        List<Vector3> dirtPositions = new List<Vector3>();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var cellPos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(cellPos);
            if (tile != null && tile.name == dirtTileName)
            {
                // Convert cell position to world position (center of tile)
                Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                dirtPositions.Add(worldPos);
            }
        }

        // Get all plants from the MainGardenManager
        MainGardenManager gardenManager = FindFirstObjectByType<MainGardenManager>();
        if (gardenManager == null || gardenManager.plants == null)
        {
            Debug.LogWarning("[PlantRandomizer] No MainGardenManager or plants found.");
            return;
        }

        Plant[] plants = gardenManager.plants;

        if (dirtPositions.Count < plants.Length)
        {
            Debug.LogWarning($"[PlantRandomizer] Not enough dirt tiles ({dirtPositions.Count}) for {plants.Length} plants.");
            return;
        }

        // Shuffle the dirt positions using Fisher-Yates
        for (int i = dirtPositions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector3 temp = dirtPositions[i];
            dirtPositions[i] = dirtPositions[j];
            dirtPositions[j] = temp;
        }

        // Assign each plant to a unique shuffled dirt tile position
        for (int i = 0; i < plants.Length; i++)
        {
            if (plants[i] != null)
            {
                Vector3 newPos = dirtPositions[i];
                newPos.z = plants[i].transform.position.z; // preserve z
                plants[i].transform.position = newPos;
            }
        }

        Debug.Log($"[PlantRandomizer] Randomized {plants.Length} plants across {dirtPositions.Count} dirt tiles.");
    }
}
