using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Fixes all SettingsCanvas UI across all scenes:
/// 1. Sets all Image types to Simple
/// 2. Sets targetGraphic on all Buttons to their Image
/// 3. Sets targetGraphic on all Sliders to their handle Image
/// 4. Ensures Resume button position is correct per scene
/// </summary>
public class FixAllCanvasUI
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

            int imagesFixed = 0;
            int buttonsFixed = 0;
            int slidersFixed = 0;

            // Fix ALL images to Simple type
            Image[] allImages = canvasGO.GetComponentsInChildren<Image>(true);
            foreach (var img in allImages)
            {
                SerializedObject imgSo = new SerializedObject(img);
                SerializedProperty typeProp = imgSo.FindProperty("m_Type");
                if (typeProp != null && typeProp.intValue != (int)Image.Type.Simple)
                {
                    typeProp.intValue = (int)Image.Type.Simple;
                    imgSo.ApplyModifiedPropertiesWithoutUndo();
                    imagesFixed++;
                }
            }

            // Fix all Button targetGraphics
            Button[] buttons = canvasGO.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    SerializedObject btnSo = new SerializedObject(btn);
                    SerializedProperty tgProp = btnSo.FindProperty("m_TargetGraphic");
                    if (tgProp != null)
                    {
                        tgProp.objectReferenceValue = img;
                        btnSo.ApplyModifiedPropertiesWithoutUndo();
                        buttonsFixed++;
                    }
                }
            }

            // Fix all Slider targetGraphics (should point to handle image)
            Slider[] sliders = canvasGO.GetComponentsInChildren<Slider>(true);
            foreach (var slider in sliders)
            {
                if (slider.handleRect != null)
                {
                    Image handleImg = slider.handleRect.GetComponent<Image>();
                    if (handleImg != null)
                    {
                        SerializedObject sliderSo = new SerializedObject(slider);
                        SerializedProperty tgProp = sliderSo.FindProperty("m_TargetGraphic");
                        if (tgProp != null)
                        {
                            tgProp.objectReferenceValue = handleImg;
                            sliderSo.ApplyModifiedPropertiesWithoutUndo();
                            slidersFixed++;
                        }
                    }
                }
            }

            // Fix Resume button position
            Transform resumeT = canvasGO.transform.Find("SettingsPanel/ResumeBtn");
            if (resumeT != null)
            {
                RectTransform rt = resumeT.GetComponent<RectTransform>();
                SerializedObject rtSo = new SerializedObject(rt);
                SerializedProperty anchoredPosProp = rtSo.FindProperty("m_AnchoredPosition");
                if (anchoredPosProp != null)
                {
                    float yPos = isGarden ? -475f : -560f;
                    anchoredPosProp.vector2Value = new Vector2(0, yPos);
                    rtSo.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            result += $"{scene.name}: images={imagesFixed}, buttons={buttonsFixed}, sliders={slidersFixed}\n";
        }

        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);
        return result;
    }
}
