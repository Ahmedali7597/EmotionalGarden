using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class TestFindCanvas
{
    public static string Execute()
    {
        // Open GardenScene
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        string result = $"Scene: {scene.name}\n";

        // Method 1: Resources.FindObjectsOfTypeAll
        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        result += $"Total canvases found (Resources): {allCanvases.Length}\n";
        foreach (var c in allCanvases)
        {
            result += $"  Canvas: '{c.gameObject.name}', scene='{c.gameObject.scene.name}', active={c.gameObject.activeSelf}, sceneLoaded={c.gameObject.scene.isLoaded}\n";
        }

        // Method 2: Scene root objects
        result += "\nRoot objects:\n";
        foreach (var root in scene.GetRootGameObjects())
        {
            result += $"  '{root.name}' active={root.activeSelf}\n";
            if (root.name == "SettingsCanvas")
            {
                var wiring = root.GetComponent<SettingsCanvasWiring>();
                result += $"    Has SettingsCanvasWiring: {wiring != null}\n";
                
                // Check button references
                if (wiring != null)
                {
                    var so = new SerializedObject(wiring);
                    var resumeProp = so.FindProperty("resumeBtn");
                    var reselectProp = so.FindProperty("reselectEmotionBtn");
                    var gardenProp = so.FindProperty("mainGardenBtn");
                    var gameSoundProp = so.FindProperty("gameSoundSlider");
                    var bgSoundProp = so.FindProperty("bgSoundSlider");
                    
                    result += $"    resumeBtn ref: {(resumeProp.objectReferenceValue != null ? resumeProp.objectReferenceValue.name : "NULL")}\n";
                    result += $"    reselectEmotionBtn ref: {(reselectProp.objectReferenceValue != null ? reselectProp.objectReferenceValue.name : "NULL")}\n";
                    result += $"    mainGardenBtn ref: {(gardenProp.objectReferenceValue != null ? gardenProp.objectReferenceValue.name : "NULL")}\n";
                    result += $"    gameSoundSlider ref: {(gameSoundProp.objectReferenceValue != null ? gameSoundProp.objectReferenceValue.name : "NULL")}\n";
                    result += $"    bgSoundSlider ref: {(bgSoundProp.objectReferenceValue != null ? bgSoundProp.objectReferenceValue.name : "NULL")}\n";
                }

                // Check buttons have targetGraphic
                var buttons = root.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    var img = btn.GetComponent<Image>();
                    result += $"    Button '{btn.name}': targetGraphic={(btn.targetGraphic != null ? btn.targetGraphic.name : "NULL")}, hasImage={img != null}, imgType={img?.type}\n";
                }
            }
        }

        return result;
    }
}
