using System;
using UnityEngine;
using UnityEditor;

public class ConfigureUISprites
{
    public static string Execute()
    {
        // List of all UI textures we need as sprites
        string[] texturePaths = new string[]
        {
            "Assets/UI/ButtonsIcons/IconButton_Small_Green_Circle.png",
            "Assets/UI/ButtonsIcons/IconButton_Small_Blank_Circle.png",
            "Assets/UI/ButtonsText/PremadeButtons_Resume.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Background.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Button.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Frame.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Scroller.png",
        };

        int configured = 0;
        foreach (var path in texturePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.SaveAndReimport();
                    configured++;
                    Debug.Log($"Configured as Sprite: {path}");
                }
                else
                {
                    Debug.Log($"Already Sprite: {path}");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find texture at: {path}");
            }
        }

        AssetDatabase.Refresh();
        return $"Configured {configured} textures as Sprites. All {texturePaths.Length} textures checked.";
    }
}
