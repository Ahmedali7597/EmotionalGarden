using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CleanBuildScenes
{
    public static void Execute()
    {
        List<EditorBuildSettingsScene> uniqueScenes = new List<EditorBuildSettingsScene>();
        HashSet<string> paths = new HashSet<string>();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!paths.Contains(scene.path))
            {
                paths.Add(scene.path);
                uniqueScenes.Add(scene);
            }
        }

        EditorBuildSettings.scenes = uniqueScenes.ToArray();
        Debug.Log($"Cleaned Build Settings. Total scenes: {uniqueScenes.Count}");
    }
}
