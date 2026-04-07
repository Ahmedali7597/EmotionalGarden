using System;
using UnityEngine;
using UnityEditor;

public class InspectSpriteAssets
{
    public static string Execute()
    {
        string[] paths = new string[]
        {
            "Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png",
            "Assets/UI/ButtonsText/ButtonText_Small_Blank_Round.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Background.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Button.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Frame.png",
            "Assets/UI/Sliders/SlimSlider_Blue_Scroller.png",
            "Assets/UI/Sliders/WideSlider_Blue_Background.png",
            "Assets/UI/Sliders/WideSlider_Blue_Button.png",
            "Assets/UI/Sliders/WideSlider_Blue_Frame.png",
            "Assets/UI/Sliders/WideSlider_Blue_Scroller.png",
        };

        string result = "";
        foreach (var path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                string border = sprite != null ? sprite.border.ToString() : "N/A";
                string texSize = tex != null ? $"{tex.width}x{tex.height}" : "N/A";
                result += $"{path}\n  Type: {importer.textureType}, SpriteMode: {importer.spriteImportMode}, Border: {border}, Size: {texSize}\n";
            }
            else
            {
                result += $"{path} — NOT FOUND\n";
            }
        }
        return result;
    }
}
