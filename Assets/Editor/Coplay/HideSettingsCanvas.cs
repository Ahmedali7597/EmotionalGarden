using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HideSettingsCanvas
{
    public static string Execute()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            if (root.name == "SettingsCanvas")
            {
                root.SetActive(false);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                return "SettingsCanvas hidden and scene saved.";
            }
        }
        return "SettingsCanvas not found.";
    }
}
