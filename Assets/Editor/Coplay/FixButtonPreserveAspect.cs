using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Turns off preserveAspect on all buttons in SettingsCanvas across all scenes
/// so buttons fill their full RectTransform size and text fits properly.
/// </summary>
public class FixButtonPreserveAspect
{
    public static string Execute()
    {
        var scenePaths = new string[]
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
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            GameObject canvasGO = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "SettingsCanvas")
                {
                    canvasGO = root;
                    break;
                }
            }

            if (canvasGO == null)
            {
                result += $"{scene.name}: No SettingsCanvas\n";
                continue;
            }

            int fixed_count = 0;
            Button[] buttons = canvasGO.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null && img.preserveAspect)
                {
                    img.preserveAspect = false;
                    EditorUtility.SetDirty(img);
                    fixed_count++;
                }
            }

            // Make sure canvas is inactive
            if (canvasGO.activeSelf)
            {
                canvasGO.SetActive(false);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            result += $"{scene.name}: Fixed {fixed_count} buttons\n";
        }

        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);
        return result;
    }
}
