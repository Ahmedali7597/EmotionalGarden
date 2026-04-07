using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FixButtonTargetGraphics
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

            int fixCount = 0;

            // Fix all buttons using SerializedObject for proper serialization
            Button[] buttons = canvasGO.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    // Use SerializedObject to properly set targetGraphic
                    SerializedObject so = new SerializedObject(btn);
                    SerializedProperty targetGraphicProp = so.FindProperty("m_TargetGraphic");
                    if (targetGraphicProp != null && targetGraphicProp.objectReferenceValue != img)
                    {
                        targetGraphicProp.objectReferenceValue = img;
                        so.ApplyModifiedPropertiesWithoutUndo();
                        fixCount++;
                    }

                    // Also fix Image type if needed
                    if (img.type == Image.Type.Filled && img.sprite != null)
                    {
                        SerializedObject imgSo = new SerializedObject(img);
                        SerializedProperty typeProp = imgSo.FindProperty("m_Type");
                        if (typeProp != null)
                        {
                            typeProp.intValue = (int)Image.Type.Sliced;
                            imgSo.ApplyModifiedPropertiesWithoutUndo();
                        }
                    }
                }
            }

            // Fix Resume button position for garden scene
            if (isGarden)
            {
                Transform resumeT = canvasGO.transform.Find("SettingsPanel/ResumeBtn");
                if (resumeT != null)
                {
                    RectTransform rt = resumeT.GetComponent<RectTransform>();
                    SerializedObject rtSo = new SerializedObject(rt);
                    SerializedProperty anchoredPosProp = rtSo.FindProperty("m_AnchoredPosition");
                    if (anchoredPosProp != null)
                    {
                        anchoredPosProp.vector2Value = new Vector2(0, -475f);
                        rtSo.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            result += $"{scene.name}: Fixed {fixCount} button targetGraphics\n";
        }

        // Re-open GardenScene
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);

        return result;
    }
}
