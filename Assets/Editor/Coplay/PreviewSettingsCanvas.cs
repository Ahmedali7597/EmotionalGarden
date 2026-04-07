using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PreviewSettingsCanvas
{
    public static string Execute()
    {
        // Open GardenScene
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);

        // Find the SettingsCanvas (inactive)
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            if (root.name == "SettingsCanvas")
            {
                // Temporarily enable it for screenshot
                root.SetActive(true);
                EditorSceneManager.MarkSceneDirty(scene);
                return "SettingsCanvas enabled for preview in GardenScene";
            }
        }
        return "SettingsCanvas not found";
    }
}
