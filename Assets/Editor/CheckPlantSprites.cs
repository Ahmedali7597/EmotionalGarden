using UnityEngine;
using UnityEditor;

public class CheckPlantSprites
{
    [MenuItem("Tools/Check Plant Sprites")]
    public static void Execute()
    {
        // Check all sprite assets from the Plants.png files
        string[] paths = new string[]
        {
            "Assets/Plants/PNG/Plants.png",
            "Assets/Main Garden/Farm tile/PNG/Plants.png",
            "Assets/Main Garden/Farm tile/Tiled_files/Plants.png"
        };

        foreach (string path in paths)
        {
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            Debug.Log($"[PlantSprites] === {path} === ({sprites.Length} sub-assets)");
            foreach (Object obj in sprites)
            {
                if (obj is Sprite sprite)
                {
                    Debug.Log($"[PlantSprites]   Sprite: '{sprite.name}' size=({sprite.rect.width}x{sprite.rect.height})");
                }
                else if (obj is Texture2D tex)
                {
                    Debug.Log($"[PlantSprites]   Texture: '{tex.name}' ({tex.width}x{tex.height}), spriteMode={AssetDatabase.LoadAssetAtPath<TextureImporter>(path)?.spriteImportMode}");
                }
            }
        }

        // Also check what the growthSprites arrays currently reference on each plant
        Plant[] plants = Object.FindObjectsByType<Plant>(FindObjectsSortMode.None);
        foreach (Plant p in plants)
        {
            Debug.Log($"[PlantSprites] Plant '{p.gameObject.name}': plantSprite={(p.plantSprite != null ? p.plantSprite.name : "NULL")}, growthSprites.Length={p.growthSprites?.Length ?? 0}");
            if (p.growthSprites != null)
            {
                for (int i = 0; i < p.growthSprites.Length; i++)
                {
                    Debug.Log($"[PlantSprites]   growthSprites[{i}] = {(p.growthSprites[i] != null ? p.growthSprites[i].name : "NULL")}");
                }
            }
            // Check if there's a SpriteRenderer on the object
            var sr = p.GetComponent<SpriteRenderer>();
            Debug.Log($"[PlantSprites]   Has SpriteRenderer: {sr != null}");
        }
    }
}
