using System;
using UnityEngine;
using UnityEditor;

public class FixSpriteImports
{
    public static string Execute()
    {
        string result = "";
        int fixed_count = 0;

        // All sprites that need to be Single mode
        string[] singleModeSprites = new string[]
        {
            "Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png",
            "Assets/UI/ButtonsText/ButtonText_Small_Blank_Round.png",
            "Assets/UI/ButtonsIcons/IconButton_Small_Green_Circle.png",
            "Assets/UI/ButtonsText/PremadeButtons_Resume.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Background.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Button.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Frame.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Scroller.png",
            "Assets/UI/Sliders/WideSlider_Blue_Background.png",
            "Assets/UI/Sliders/WideSlider_Blue_Button.png",
            "Assets/UI/Sliders/WideSlider_Blue_Frame.png",
            "Assets/UI/Sliders/WideSlider_Blue_Scroller.png",
        };

        foreach (var path in singleModeSprites)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    changed = true;
                }
                if (changed)
                {
                    importer.SaveAndReimport();
                    fixed_count++;
                    result += $"Fixed: {path}\n";
                }
                else
                {
                    result += $"Already OK: {path}\n";
                }
            }
            else
            {
                result += $"NOT FOUND: {path}\n";
            }
        }

        // Now set 9-slice borders on sprites that need horizontal stretching
        // Background and Frame sprites need left/right borders for proper 9-slice
        var slicedSprites = new (string path, Vector4 border)[]
        {
            // SlimSlider_Blue_Background: 1365x118, needs left/right borders
            ("Assets/UI/Sliders/SlimSlider_Blue_Background.png", new Vector4(50, 0, 50, 0)),
            // SlimSlider_Blue_Frame: 1467x213, needs left/right borders
            ("Assets/UI/Sliders/SlimSlider_Blue_Frame.png", new Vector4(80, 0, 80, 0)),
            // WideSlider_Blue_Background: 1432x202
            ("Assets/UI/Sliders/WideSlider_Blue_Background.png", new Vector4(80, 0, 80, 0)),
            // WideSlider_Blue_Frame: 1536x310
            ("Assets/UI/Sliders/WideSlider_Blue_Frame.png", new Vector4(100, 0, 100, 0)),
            // Button text sprites need all-around borders for proper scaling
            ("Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png", new Vector4(40, 40, 40, 40)),
            ("Assets/UI/ButtonsText/ButtonText_Small_Blank_Round.png", new Vector4(40, 40, 40, 40)),
        };

        foreach (var (path, border) in slicedSprites)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = border;
                importer.SaveAndReimport();
                result += $"Set border {border} on: {path}\n";
                fixed_count++;
            }
        }

        AssetDatabase.Refresh();
        result += $"\nTotal fixed: {fixed_count}";
        return result;
    }
}
