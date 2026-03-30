using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AddMainGardenToBuild
{
    public static void Execute()
    {
        string mainGardenPath = "Assets/Minigame/MainGarden/MainGarden.unity";
        
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        
        bool found = false;
        foreach (var scene in scenes)
        {
            if (scene.path == mainGardenPath)
            {
                found = true;
                break;
            }
        }
        
        if (!found)
        {
            scenes.Add(new EditorBuildSettingsScene(mainGardenPath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("Added MainGarden to Build Settings.");
        }
        else
        {
            Debug.Log("MainGarden is already in Build Settings.");
        }
    }
}
