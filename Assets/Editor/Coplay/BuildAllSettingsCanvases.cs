using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Builds SettingsCanvas into all game scenes in one go.
/// Each scene is opened, modified, saved, then the next is opened.
/// </summary>
public class BuildAllSettingsCanvases
{
    private const string BTN_GREEN_ROUND = "Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png";
    private const string BTN_BLANK_ROUND = "Assets/UI/ButtonsText/ButtonText_Small_Blank_Round.png";
    private const string PREMADE_RESUME  = "Assets/UI/ButtonsText/PremadeButtons_Resume.png";
    private const string SLIDER_BLUE_BG       = "Assets/UI/Sliders/SlimSlider_Blue_Background.png";
    private const string SLIDER_BLUE_BUTTON   = "Assets/UI/Sliders/SlimSlider_Blue_Button.png";
    private const string SLIDER_BLUE_FRAME    = "Assets/UI/Sliders/SlimSlider_Blue_Frame.png";

    public static string Execute()
    {
        var scenes = new (string path, bool isGarden)[]
        {
            ("Assets/Minigame/CloudGame/CloudGame.unity", false),
            ("Assets/Minigame/DarkGame/DarkGame.unity", false),
            ("Assets/Minigame/PuzzleGame/PuzzleGame.unity", false),
            ("Assets/Minigame/RunGame/RunGame.unity", false),
        };

        string result = "";

        foreach (var (path, isGarden) in scenes)
        {
            // Open the scene
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            // Remove existing SettingsCanvas if any
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name == "SettingsCanvas")
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }

            // Build the canvas
            GameObject canvasGO = BuildCanvas(isGarden);

            // Move it to the correct scene
            SceneManager.MoveGameObjectToScene(canvasGO, scene);

            // Save
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            result += $"Built SettingsCanvas in: {scene.name}\n";
        }

        // Re-open GardenScene at the end
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);

        return result;
    }

    private static GameObject BuildCanvas(bool isGardenScene)
    {
        // ===== Canvas =====
        GameObject canvasGO = new GameObject("SettingsCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 900;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Add the wiring component
        SettingsCanvasWiring wiring = canvasGO.AddComponent<SettingsCanvasWiring>();

        // ===== Semi-transparent overlay =====
        GameObject overlayGO = CreateUIElement("Overlay", canvasGO.transform);
        Image overlayImage = overlayGO.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.6f);
        StretchToFill(overlayGO.GetComponent<RectTransform>());

        // ===== Settings panel =====
        GameObject panelGO = CreateUIElement("SettingsPanel", canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.12f, 0.12f, 0.22f, 0.95f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700, 750);
        panelRect.anchoredPosition = Vector2.zero;

        // ===== "Settings" title =====
        CreateText(panelGO.transform, "TitleText", "Settings",
            54, FontStyle.Bold, Color.white,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(600, 80), new Vector2(0, -50));

        // ===== Game Sound label =====
        CreateText(panelGO.transform, "GameSoundLabel", "Game Sound",
            32, FontStyle.Normal, new Color(0.9f, 0.9f, 0.9f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(600, 45), new Vector2(0, -120));

        // ===== Game Sound Slider =====
        GameObject gameSoundSlider = BuildBlueSlider(panelGO.transform, "GameSoundSlider", new Vector2(0, -175));

        // ===== Background Sound label =====
        CreateText(panelGO.transform, "BGSoundLabel", "Background Sound",
            32, FontStyle.Normal, new Color(0.9f, 0.9f, 0.9f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(600, 45), new Vector2(0, -240));

        // ===== Background Sound Slider =====
        GameObject bgSoundSlider = BuildBlueSlider(panelGO.transform, "BGSoundSlider", new Vector2(0, -295));

        // ===== Reselect Emotion button =====
        GameObject reselectBtn = BuildSpriteButton(panelGO.transform, "ReselectEmotionBtn", "Reselect Emotion",
            BTN_BLANK_ROUND, new Vector2(0, -390), new Vector2(350, 70));

        // ===== Main Garden button =====
        GameObject mainGardenBtn = BuildSpriteButton(panelGO.transform, "MainGardenBtn", "Main Garden",
            BTN_GREEN_ROUND, new Vector2(0, -475), new Vector2(350, 70));

        // ===== Resume button =====
        float resumeY = isGardenScene ? -475f : -560f;
        GameObject resumeBtn = BuildSpriteButton(panelGO.transform, "ResumeBtn", "",
            PREMADE_RESUME, new Vector2(0, resumeY), new Vector2(350, 70));

        // Wire up serialized references
        SerializedObject so = new SerializedObject(wiring);
        so.FindProperty("gameSoundSlider").objectReferenceValue = gameSoundSlider.GetComponent<Slider>();
        so.FindProperty("bgSoundSlider").objectReferenceValue = bgSoundSlider.GetComponent<Slider>();
        so.FindProperty("reselectEmotionBtn").objectReferenceValue = reselectBtn.GetComponent<Button>();
        so.FindProperty("mainGardenBtn").objectReferenceValue = mainGardenBtn.GetComponent<Button>();
        so.FindProperty("resumeBtn").objectReferenceValue = resumeBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // Start hidden
        canvasGO.SetActive(false);

        return canvasGO;
    }

    private static GameObject BuildSpriteButton(Transform parent, string name, string label,
        string spriteAssetPath, Vector2 position, Vector2 size)
    {
        GameObject btnGO = CreateUIElement(name, parent);
        Image btnImage = btnGO.AddComponent<Image>();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spriteAssetPath);
        if (sprite != null)
        {
            btnImage.sprite = sprite;
            btnImage.type = Image.Type.Sliced;
            btnImage.color = Color.white;
        }
        else
        {
            btnImage.color = new Color(0.4f, 0.4f, 0.5f);
        }

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f);
        colors.selectedColor = Color.white;
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 1f);
        btnRect.anchorMax = new Vector2(0.5f, 1f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = position;

        if (!string.IsNullOrEmpty(label))
        {
            GameObject txtGO = CreateUIElement("Text", btnGO.transform);
            Text btnText = txtGO.AddComponent<Text>();
            btnText.text = label;
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnText.fontSize = 28;
            btnText.fontStyle = FontStyle.Bold;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.horizontalOverflow = HorizontalWrapMode.Overflow;
            btnText.color = Color.white;

            Shadow shadow = txtGO.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);

            StretchToFill(txtGO.GetComponent<RectTransform>());
        }

        return btnGO;
    }

    private static GameObject BuildBlueSlider(Transform parent, string name, Vector2 position)
    {
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SLIDER_BLUE_BG);
        Sprite frameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SLIDER_BLUE_FRAME);
        Sprite handleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SLIDER_BLUE_BUTTON);

        GameObject sliderGO = CreateUIElement(name, parent);
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 1f);
        sliderRect.anchorMax = new Vector2(0.5f, 1f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(500, 50);
        sliderRect.anchoredPosition = position;

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.value = 1f;

        // Background
        GameObject bgGO = CreateUIElement("Background", sliderGO.transform);
        Image bgImg = bgGO.AddComponent<Image>();
        if (bgSprite != null) { bgImg.sprite = bgSprite; bgImg.type = Image.Type.Sliced; bgImg.color = Color.white; }
        else { bgImg.color = new Color(0.15f, 0.25f, 0.45f); }
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.3f);
        bgRect.anchorMax = new Vector2(1f, 0.7f);
        bgRect.offsetMin = new Vector2(10f, 0f);
        bgRect.offsetMax = new Vector2(-10f, 0f);

        // Fill Area
        GameObject fillAreaGO = CreateUIElement("Fill Area", sliderGO.transform);
        RectTransform fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.3f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.7f);
        fillAreaRect.offsetMin = new Vector2(12f, 0f);
        fillAreaRect.offsetMax = new Vector2(-12f, 0f);

        GameObject fillGO = CreateUIElement("Fill", fillAreaGO.transform);
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.55f, 0.95f);
        fillImg.type = Image.Type.Simple;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;

        // Handle Slide Area
        GameObject handleAreaGO = CreateUIElement("Handle Slide Area", sliderGO.transform);
        RectTransform handleAreaRect = handleAreaGO.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(20f, 0f);
        handleAreaRect.offsetMax = new Vector2(-20f, 0f);

        // Handle
        GameObject handleGO = CreateUIElement("Handle", handleAreaGO.transform);
        Image handleImg = handleGO.AddComponent<Image>();
        if (handleSprite != null) { handleImg.sprite = handleSprite; handleImg.type = Image.Type.Simple; handleImg.color = Color.white; handleImg.preserveAspect = true; }
        else { handleImg.color = new Color(0.3f, 0.6f, 1f); }
        RectTransform handleRect = handleGO.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0f, 0.5f);
        handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(45, 45);

        // Frame overlay
        GameObject frameGO = CreateUIElement("Frame", sliderGO.transform);
        Image frameImg = frameGO.AddComponent<Image>();
        if (frameSprite != null) { frameImg.sprite = frameSprite; frameImg.type = Image.Type.Sliced; frameImg.color = Color.white; }
        else { frameImg.color = Color.clear; }
        frameImg.raycastTarget = false;
        StretchToFill(frameGO.GetComponent<RectTransform>());

        // Wire slider
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;

        ColorBlock sliderColors = slider.colors;
        sliderColors.normalColor = Color.white;
        sliderColors.highlightedColor = new Color(0.9f, 0.95f, 1f);
        sliderColors.pressedColor = new Color(0.8f, 0.85f, 0.9f);
        slider.colors = sliderColors;

        return sliderGO;
    }

    private static void CreateText(Transform parent, string name, string content,
        int fontSize, FontStyle style, Color color,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 size, Vector2 position)
    {
        GameObject go = CreateUIElement(name, parent);
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.color = color;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    private static void StretchToFill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
