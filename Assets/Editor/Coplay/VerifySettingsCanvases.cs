using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VerifySettingsCanvases
{
    public static string Execute()
    {
        string[] scenePaths = new string[]
        {
            "Assets/Main Garden/Scenes/GardenScene.unity",
            "Assets/Minigame/CloudGame/CloudGame.unity",
            "Assets/Minigame/DarkGame/DarkGame.unity",
            "Assets/Minigame/PuzzleGame/PuzzleGame.unity",
            "Assets/Minigame/RunGame/RunGame.unity",
        };

        string result = "";
        foreach (var path in scenePaths)
        {
            var scene = EditorSceneManager.OpenScene(path);
            
            // Search for SettingsCanvas (including inactive)
            bool found = false;
            bool hasWiring = false;
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                if (root.name == "SettingsCanvas")
                {
                    found = true;
                    hasWiring = root.GetComponent<SettingsCanvasWiring>() != null;
                    break;
                }
            }
            
            result += $"{scene.name}: SettingsCanvas={found}, Wiring={hasWiring}\n";
        }

        // Re-open GardenScene at the end
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity");

        return result;
    }
}
