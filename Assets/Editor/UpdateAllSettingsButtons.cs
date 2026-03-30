using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class UpdateAllSettingsButtons
{
    private static readonly string iconPath = "Assets/Minigame/UI/settings_icon.png";

    public static void Execute()
    {
        // Set up icon texture import settings (Sprite type)
        SetupIconImportSettings();

        // Update each scene
        UpdateScene("Assets/Minigame/CloudGame/CloudGame.unity", 1.84f, 4.61f);
        UpdateScene("Assets/Minigame/PuzzleGame/PuzzleGame.unity", 7.5f, 4.2f);
        UpdateScene("Assets/Minigame/RunGame/RunGame.unity", -1f, -1f); // auto-detect
        UpdateScene("Assets/Minigame/DarkGame/DarkGame.unity", -1f, -1f); // auto-detect

        // Add DarkGame to Build Settings
        AddDarkGameToBuildSettings();

        Debug.Log("All SettingsBtn updated with gear icon!");
    }

    static void SetupIconImportSettings()
    {
        TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 128;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
            Debug.Log("Icon import settings updated.");
        }
    }

    static void UpdateScene(string scenePath, float posX, float posY)
    {
        var scene = EditorSceneManager.OpenScene(scenePath);

        // Auto-calculate position from camera when posX == -1
        if (posX < 0)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                float orthoSize = cam.orthographicSize;
                float aspect = cam.aspect;
                posX = orthoSize * aspect - 0.5f;
                posY = orthoSize - 0.5f;
            }
            else
            {
                posX = 7.5f;
                posY = 4.2f;
            }
        }

        // Find existing SettingsBtn or create new
        GameObject btn = GameObject.Find("SettingsBtn");
        if (btn == null)
        {
            // Check for legacy pauseBtn
            btn = GameObject.Find("pauseBtn");
        }

        if (btn == null)
        {
            // Create new
            btn = new GameObject("SettingsBtn");
        }

        btn.name = "SettingsBtn";
        btn.transform.position = new Vector3(posX, posY, -1f);
        btn.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        // SpriteRenderer setup
        SpriteRenderer sr = btn.GetComponent<SpriteRenderer>();
        if (sr == null) sr = btn.AddComponent<SpriteRenderer>();

        Sprite gearSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        sr.sprite = gearSprite;
        sr.color = Color.white;
        sr.sortingOrder = 100;

        // BoxCollider2D setup
        BoxCollider2D col = btn.GetComponent<BoxCollider2D>();
        if (col == null) col = btn.AddComponent<BoxCollider2D>();

        // SettingsButton script check
        var existingScript = btn.GetComponent("SettingsButton");
        if (existingScript == null)
        {
            var scriptType = System.Type.GetType("SettingsButton, Assembly-CSharp");
            if (scriptType != null)
            {
                btn.AddComponent(scriptType);
            }
        }

        // Remove legacy PauseButton if present
        var pauseBtn = btn.GetComponent("PauseButton");
        if (pauseBtn != null)
        {
            Object.DestroyImmediate(pauseBtn);
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Updated SettingsBtn in {scenePath} at ({posX:F1}, {posY:F1})");
    }

    static void AddDarkGameToBuildSettings()
    {
        string darkGamePath = "Assets/Minigame/DarkGame/DarkGame.unity";
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        bool found = false;
        foreach (var s in scenes)
        {
            if (s.path == darkGamePath) { found = true; break; }
        }

        if (!found)
        {
            scenes.Add(new EditorBuildSettingsScene(darkGamePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("DarkGame added to Build Settings.");
        }
    }
}
