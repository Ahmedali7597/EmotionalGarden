using UnityEditor;
using System.Collections.Generic;

public class SetupBuildScenes
{
    public static void Execute()
    {
        // All scenes that need to be in build settings
        string[] scenePaths = new string[]
        {
            "Assets/Minigame/MainGarden/MainGarden.unity",
            "Assets/Minigame/CloudGame/CloudGame.unity",
            "Assets/Minigame/PuzzleGame/PuzzleGame.unity",
            "Assets/Minigame/RunGame/RunGame.unity"
        };

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

        // Keep existing scenes
        foreach (var existing in EditorBuildSettings.scenes)
        {
            bool alreadyInList = false;
            foreach (string path in scenePaths)
            {
                if (existing.path == path)
                {
                    alreadyInList = true;
                    break;
                }
            }
            if (!alreadyInList)
            {
                buildScenes.Add(existing);
            }
        }

        // Add our scenes
        foreach (string path in scenePaths)
        {
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        UnityEngine.Debug.Log($"Build Settings updated: {buildScenes.Count} scenes registered.");
    }
}
