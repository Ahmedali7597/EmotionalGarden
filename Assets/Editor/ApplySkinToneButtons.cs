using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ApplySkinToneButtons
{
    public static void Execute()
    {
        // Refresh asset database to ensure the new images are loaded
        AssetDatabase.Refresh();

        Scene avatarScene = EditorSceneManager.OpenScene("Assets/Avatar/Scenes/AvatarSelectScene.unity", OpenSceneMode.Single);

        GameObject skinTonePanel = GameObject.Find("SkinTonePanel");
        if (skinTonePanel != null)
        {
            Transform lightBtnT = skinTonePanel.transform.Find("LightButton");
            Transform tanBtnT = skinTonePanel.transform.Find("TanButton");
            Transform blackBtnT = skinTonePanel.transform.Find("BlackButton");

            Sprite lightSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/LightButton.png");
            Sprite tanSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/TanButton.png");
            Sprite blackSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/BlackButton.png");

            // Apply to Light Button
            if (lightBtnT != null && lightSprite != null)
            {
                ApplyStyle(lightBtnT.gameObject, lightSprite, new Vector2(0, 220));
            }

            // Apply to Tan Button
            if (tanBtnT != null && tanSprite != null)
            {
                ApplyStyle(tanBtnT.gameObject, tanSprite, new Vector2(0, 0));
            }

            // Apply to Black Button
            if (blackBtnT != null && blackSprite != null)
            {
                ApplyStyle(blackBtnT.gameObject, blackSprite, new Vector2(0, -220));
            }
        }

        EditorSceneManager.SaveScene(avatarScene);
        Debug.Log("Skin Tone buttons updated to match the new style!");
    }

    private static void ApplyStyle(GameObject btnGO, Sprite sprite, Vector2 position)
    {
        Image img = btnGO.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Simple;
            img.preserveAspect = true;
            img.color = Color.white; // Reset color tint since the image has the color baked in
        }

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        // Make them slightly larger as requested (e.g., 550x183 to match gender buttons)
        rt.sizeDelta = new Vector2(550, 183);
        rt.anchoredPosition = position;

        // Hide the old text label since the text is baked into the image
        Transform label = btnGO.transform.Find("Label");
        if (label != null)
        {
            label.gameObject.SetActive(false);
        }
        
        // Also hide any direct Text component if it exists
        Text txt = btnGO.GetComponent<Text>();
        if (txt != null)
        {
            txt.enabled = false;
        }

        // Remove shadow if it exists, as the new images have a flat vector look
        Shadow shadow = btnGO.GetComponent<Shadow>();
        if (shadow != null)
        {
            Object.DestroyImmediate(shadow);
        }
    }
}
