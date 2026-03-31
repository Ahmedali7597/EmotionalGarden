using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixPlantSprites
{
    public static void Execute()
    {
        // Target only Plants 6-9
        string[] plantNames = { "Plant6", "Plant7", "Plant8", "Plant9" };
        List<Plant> fixedPlants = new List<Plant>();

        foreach (string name in plantNames)
        {
            GameObject go = GameObject.Find(name);
            if (go == null)
            {
                Debug.LogWarning($"[FixPlants] Could not find '{name}'");
                continue;
            }

            Plant plant = go.GetComponent<Plant>();
            if (plant == null)
            {
                Debug.LogWarning($"[FixPlants] '{name}' has no Plant component");
                continue;
            }

            // Add SpriteRenderer if missing
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = go.AddComponent<SpriteRenderer>();
                Debug.Log($"[FixPlants] Added SpriteRenderer to '{name}'");
            }

            // Wire the plantSprite field to the SpriteRenderer
            plant.plantSprite = sr;

            // Set the initial sprite to growthSprites[0] so the plant is visible
            if (plant.growthSprites != null && plant.growthSprites.Length > 0 && plant.growthSprites[0] != null)
            {
                sr.sprite = plant.growthSprites[0];
                Debug.Log($"[FixPlants] Set initial sprite for '{name}' to '{plant.growthSprites[0].name}'");
            }

            // Set sorting order so plants render in front of the tilemap
            sr.sortingOrder = 5;

            // Mark dirty
            EditorUtility.SetDirty(plant);
            EditorUtility.SetDirty(sr);
            fixedPlants.Add(plant);
        }

        // Update MainGardenManager.plants array to include ALL plants
        MainGardenManager mgm = Object.FindFirstObjectByType<MainGardenManager>();
        if (mgm != null)
        {
            Plant[] allPlants = Object.FindObjectsByType<Plant>(FindObjectsSortMode.None);
            mgm.plants = allPlants;
            EditorUtility.SetDirty(mgm);
            Debug.Log($"[FixPlants] Updated MainGardenManager.plants array to {allPlants.Length} plants");
        }

        // Save the scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        Debug.Log($"[FixPlants] Fixed {fixedPlants.Count} plants (6-9). Scene saved.");
    }
}
