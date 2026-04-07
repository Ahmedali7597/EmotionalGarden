using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Fixes all SettingsCanvas buttons across all scenes:
/// 1. Sets targetGraphic on each Button to its own Image component
/// 2. Ensures Image.type is Sliced (not Filled)
/// 3. In GardenScene, sets Resume button default position to -475 (same as MainGardenBtn)
/// 4. In minigame scenes, sets Resume button default position to -560
/// </summary>
public class FixSettingsCanvasButtons
{
    public static string Execute()
    {
        var scenes = new (string path, bool isGarden)[]
        {
            ("Assets/Main Garden/Scenes/GardenScene.unity", true),
            ("Assets/Minigame/CloudGame/CloudGame.unity", false),
            ("Assets/Minigame/DarkGame/DarkGame.unity", false),
            ("Assets/Minigame/PuzzleGame/PuzzleGame.unity", false),
            ("Assets/Minigame/RunGame/RunGame.unity", false),
        };

        string result = "";

        foreach (var (path, isGarden) in scenes)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool modified = false;

            // Find SettingsCanvas (including inactive)
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
                result += $"{scene.name}: No SettingsCanvas found!\n";
                continue;
            }

            // Fix all buttons — set targetGraphic to their own Image
            Button[] buttons = canvasGO.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    // Fix targetGraphic
                    if (btn.targetGraphic != img)
                    {
                        btn.targetGraphic = img;
                        modified = true;
                    }

                    // Fix Image type — should be Sliced, not Filled
                    if (img.type != Image.Type.Sliced && img.sprite != null)
                    {
                        img.type = Image.Type.Sliced;
                        modified = true;
                    }
                }
            }

            // Fix Resume button position
            Transform resumeTransform = canvasGO.transform.Find("SettingsPanel/ResumeBtn");
            Transform gardenBtnTransform = canvasGO.transform.Find("SettingsPanel/MainGardenBtn");

            if (resumeTransform != null)
            {
                RectTransform resumeRect = resumeTransform.GetComponent<RectTransform>();
                if (isGarden)
                {
                    // In garden: Resume goes to MainGardenBtn's position (-475)
                    // The wiring script also does this at runtime, but set it in the scene too
                    resumeRect.anchoredPosition = new Vector2(0, -475f);
                    modified = true;
                }
                else
                {
                    // In minigames: Resume goes below MainGardenBtn
                    resumeRect.anchoredPosition = new Vector2(0, -560f);
                    modified = true;
                }
            }

            if (modified)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            int btnCount = buttons.Length;
            result += $"{scene.name}: Fixed {btnCount} buttons, isGarden={isGarden}\n";
        }

        // Re-open GardenScene
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);

        return result;
    }
}
