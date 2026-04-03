using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Utility class that programmatically creates a shared Settings screen.
/// Call SettingsUI.Toggle() from any scene.
/// Sound values are saved to PlayerPrefs for later use with AudioSource.
/// </summary>
public static class SettingsUI
{
    private static GameObject canvasGO;
    public static bool isOpen = false;

    // Current volume values (0~1)
    private static float gameVolume = 1f;
    private static float bgVolume = 1f;

    private const string GAME_VOLUME_KEY = "GameSoundVolume";
    private const string BG_VOLUME_KEY = "BGSoundVolume";

    /// <summary>
    /// Toggle Settings screen (open/close)
    /// </summary>
    public static void Toggle()
    {
        if (isOpen)
            Hide();
        else
            Show();
    }

    public static void Show()
    {
        if (isOpen) return;
        isOpen = true;

        // Load saved volume from PlayerPrefs
        gameVolume = PlayerPrefs.GetFloat(GAME_VOLUME_KEY, 1f);
        bgVolume = PlayerPrefs.GetFloat(BG_VOLUME_KEY, 1f);

        Time.timeScale = 0f;
        CreateUI();
    }

    public static void Hide()
    {
        if (!isOpen) return;
        isOpen = false;

        // Save volume values
        PlayerPrefs.SetFloat(GAME_VOLUME_KEY, gameVolume);
        PlayerPrefs.SetFloat(BG_VOLUME_KEY, bgVolume);
        PlayerPrefs.Save();

        if (canvasGO != null)
        {
            Object.Destroy(canvasGO);
            canvasGO = null;
        }

        Time.timeScale = 1f;
    }

    /// <summary>
    /// Current Game Sound volume (0~1)
    /// </summary>
    public static float GetGameVolume()
    {
        return PlayerPrefs.GetFloat(GAME_VOLUME_KEY, 1f);
    }

    /// <summary>
    /// Current Background Sound volume (0~1)
    /// </summary>
    public static float GetBGVolume()
    {
        return PlayerPrefs.GetFloat(BG_VOLUME_KEY, 1f);
    }

    /// <summary>
    /// Applies the current volume settings to all AudioSources in the scene.
    /// Background sound (looping) uses BGVolume, others use GameVolume.
    /// </summary>
    public static void ApplyVolumeToActiveSources()
    {
        float bgVol = GetBGVolume();
        float gameVol = GetGameVolume();

        AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (source.loop)
            {
                source.volume = bgVol;
            }
            else
            {
                source.volume = gameVol;
            }
        }
    }

    private static bool IsInGardenScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == SceneFlow.GardenScene || sceneName == "GardenScene";
    }

    private static void CreateUI()
    {
        // ===== Canvas =====
        canvasGO = new GameObject("SettingsCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 900;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== EventSystem check =====
        EnsureEventSystem();

        // ===== Semi-transparent overlay (blocks touch) =====
        GameObject bgGO = CreateUIElement("Overlay", canvasGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.6f);
        StretchToFill(bgGO.GetComponent<RectTransform>());

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

        // ===== Game Sound =====
        CreateText(panelGO.transform, "GameSoundLabel", "Game Sound",
            32, FontStyle.Normal, new Color(0.9f, 0.9f, 0.9f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(600, 45), new Vector2(0, -120));

        CreateSlider(panelGO.transform, "GameSoundSlider",
            new Vector2(0, -165), gameVolume, (value) =>
            {
                gameVolume = value;
                PlayerPrefs.SetFloat(GAME_VOLUME_KEY, value);
                ApplyVolumeToActiveSources();
            });

        // ===== Background Sound =====
        CreateText(panelGO.transform, "BGSoundLabel", "Background Sound",
            32, FontStyle.Normal, new Color(0.9f, 0.9f, 0.9f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(600, 45), new Vector2(0, -225));

        CreateSlider(panelGO.transform, "BGSoundSlider",
            new Vector2(0, -270), bgVolume, (value) =>
            {
                bgVolume = value;
                PlayerPrefs.SetFloat(BG_VOLUME_KEY, value);
                ApplyVolumeToActiveSources();
            });

        // ===== Reselect Emotion button =====
        CreateButton(panelGO.transform, "ReselectEmotionBtn", "Reselect Emotion",
            new Color(0.6f, 0.4f, 0.7f), new Vector2(0, -360), () =>
            {
                isOpen = false;
                PlayerPrefs.SetFloat(GAME_VOLUME_KEY, gameVolume);
                PlayerPrefs.SetFloat(BG_VOLUME_KEY, bgVolume);
                PlayerPrefs.Save();
                if (canvasGO != null) Object.Destroy(canvasGO);
                canvasGO = null;
                Time.timeScale = 1f;
                SceneFlow.GoToEmotionSelect();
            });

        // ===== Main Garden button (only show when NOT in garden scene) =====
        if (!IsInGardenScene())
        {
            CreateButton(panelGO.transform, "MainGardenBtn", "Main Garden",
                new Color(0.2f, 0.55f, 0.3f), new Vector2(0, -450), () =>
                {
                    isOpen = false;
                    PlayerPrefs.SetFloat(GAME_VOLUME_KEY, gameVolume);
                    PlayerPrefs.SetFloat(BG_VOLUME_KEY, bgVolume);
                    PlayerPrefs.Save();
                    if (canvasGO != null) Object.Destroy(canvasGO);
                    canvasGO = null;
                    Time.timeScale = 1f;
                    MiniGameLauncher.ReturnToGarden();
                });
        }

        // ===== Resume button (close) =====
        float resumeY = IsInGardenScene() ? -450f : -540f;
        CreateButton(panelGO.transform, "ResumeBtn", "Resume",
            new Color(0.45f, 0.45f, 0.55f), new Vector2(0, resumeY), () =>
            {
                Hide();
            });
    }

    // ===== UI helper methods =====

    private static void CreateSlider(Transform parent, string name,
        Vector2 position, float initialValue, UnityEngine.Events.UnityAction<float> onValueChanged)
    {
        GameObject sliderGO = CreateUIElement(name, parent);
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 1f);
        sliderRect.anchorMax = new Vector2(0.5f, 1f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(500, 40);
        sliderRect.anchoredPosition = position;

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;

        // Background (track)
        GameObject bgGO = CreateUIElement("Background", sliderGO.transform);
        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.25f, 0.35f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.25f);
        bgRect.anchorMax = new Vector2(1f, 0.75f);
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        // Fill Area
        GameObject fillAreaGO = CreateUIElement("Fill Area", sliderGO.transform);
        RectTransform fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.anchoredPosition = Vector2.zero;

        GameObject fillGO = CreateUIElement("Fill", fillAreaGO.transform);
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(0.35f, 0.75f, 0.45f);
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        StretchToFill(fillRect);

        // Handle Slide Area
        GameObject handleAreaGO = CreateUIElement("Handle Slide Area", sliderGO.transform);
        RectTransform handleAreaRect = handleAreaGO.GetComponent<RectTransform>();
        StretchToFill(handleAreaRect);

        // Handle
        GameObject handleGO = CreateUIElement("Handle", handleAreaGO.transform);
        Image handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;
        RectTransform handleRect = handleGO.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(40, 40);

        // Connect slider references
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;

        ColorBlock colors = slider.colors;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        slider.colors = colors;

        slider.onValueChanged.AddListener(onValueChanged);
    }

    private static void CreateButton(Transform parent, string name, string label,
        Color bgColor, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = CreateUIElement(name, parent);
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = bgColor;

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.highlightedColor = bgColor * 1.2f;
        colors.pressedColor = bgColor * 0.8f;
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 1f);
        btnRect.anchorMax = new Vector2(0.5f, 1f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(400, 65);
        btnRect.anchoredPosition = position;

        GameObject txtGO = CreateUIElement("Text", btnGO.transform);
        Text btnText = txtGO.AddComponent<Text>();
        btnText.text = label;
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 32;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        StretchToFill(txtGO.GetComponent<RectTransform>());

        btn.onClick.AddListener(onClick);
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

    private static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
    }
}
