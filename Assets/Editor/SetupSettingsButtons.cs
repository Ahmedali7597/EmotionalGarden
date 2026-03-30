using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupSettingsButtons
{
    // Add Settings button to PuzzleGame
    public static void SetupPuzzleGame()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Minigame/PuzzleGame/PuzzleGame.unity");
        CreateSettingsButton(7.5f, 4.2f);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("PuzzleGame: SettingsBtn added and saved.");
    }

    // Add Settings button to RunGame
    public static void SetupRunGame()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Minigame/RunGame/RunGame.unity");
        // Position based on camera bounds
        Camera cam = Camera.main;
        float orthoSize = cam != null ? cam.orthographicSize : 5f;
        float aspect = cam != null ? cam.aspect : 16f / 9f;
        float x = orthoSize * aspect - 0.5f; // Top-right
        float y = orthoSize - 0.5f;
        CreateSettingsButton(x, y);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("RunGame: SettingsBtn added and saved.");
    }

    private static void CreateSettingsButton(float posX, float posY)
    {
        // Remove existing SettingsBtn if present
        GameObject existing = GameObject.Find("SettingsBtn");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        GameObject btn = new GameObject("SettingsBtn");
        btn.transform.position = new Vector3(posX, posY, -1f); // z=-1 to render above other objects
        btn.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // SpriteRenderer
        SpriteRenderer sr = btn.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");
        sr.color = new Color(0.14f, 0.30f, 0.85f, 1f);
        sr.sortingOrder = 100; // Render above other sprites

        // BoxCollider2D
        btn.AddComponent<BoxCollider2D>();

        // SettingsButton script
        var script = btn.AddComponent(System.Type.GetType("SettingsButton, Assembly-CSharp")) as MonoBehaviour;
        if (script == null)
        {
            Debug.LogError("SettingsButton script not found! Make sure it compiled.");
        }

        Debug.Log($"SettingsBtn created at ({posX}, {posY})");
    }
}
