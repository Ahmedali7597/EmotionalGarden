using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Utility class that programmatically creates a shared EndGame screen.
/// Call EndGameUI.Show(true/false) from any scene.
/// 
/// Sprites are loaded from Resources at runtime for mobile compatibility.
/// Place the required sprites in Assets/Resources/UI/ folder.
/// Falls back to AssetDatabase in Editor.
/// </summary>
public static class EndGameUI
{
    private static GameObject canvasGO;
    public static bool isShowing = false;

    // Sprite paths
    private const string BTN_GREEN_ROUND_ASSET = "Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png";

    /// <summary>
    /// Displays the EndGame screen.
    /// </summary>
    /// <param name="success">true for Success, false for Fail</param>
    public static void Show(bool success)
    {
        if (isShowing) return;
        isShowing = true;
        Time.timeScale = 0f;

        CreateUI(success);
    }

    /// <summary>
    /// Hides and destroys the EndGame screen if showing.
    /// </summary>
    public static void Hide()
    {
        if (!isShowing) return;
        isShowing = false;
        Time.timeScale = 1f;

        if (canvasGO != null)
        {
            Object.Destroy(canvasGO);
            canvasGO = null;
        }
    }

    private static void CreateUI(bool success)
    {
        // ===== Canvas (portrait mobile reference) =====
        canvasGO = new GameObject("EndGameCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== EventSystem check =====
        EnsureEventSystem();

        // ===== Semi-transparent background =====
        GameObject bgGO = CreateUIElement("Background", canvasGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);
        StretchToFill(bgGO.GetComponent<RectTransform>());

        // ===== Center panel =====
        GameObject panelGO = CreateUIElement("Panel", canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700, 550);
        panelRect.anchoredPosition = Vector2.zero;

        // ===== Result text =====
        GameObject textGO = CreateUIElement("ResultText", panelGO.transform);
        Text resultText = textGO.AddComponent<Text>();
        resultText.text = success ? "Success!" : "Fail...";
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resultText.fontSize = 80;
        resultText.fontStyle = FontStyle.Bold;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.horizontalOverflow = HorizontalWrapMode.Overflow;
        resultText.verticalOverflow = VerticalWrapMode.Overflow;
        resultText.color = success ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f);
        resultText.raycastTarget = false;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.7f);
        textRect.anchorMax = new Vector2(0.5f, 0.7f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(600, 120);
        textRect.anchoredPosition = Vector2.zero;

        // ===== Subtitle text =====
        GameObject subTextGO = CreateUIElement("SubText", panelGO.transform);
        Text subText = subTextGO.AddComponent<Text>();
        subText.text = success ? "Well Done!" : "Try Again Next Time";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 36;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.horizontalOverflow = HorizontalWrapMode.Overflow;
        subText.color = new Color(0.8f, 0.8f, 0.8f);
        subText.raycastTarget = false;

        RectTransform subTextRect = subTextGO.GetComponent<RectTransform>();
        subTextRect.anchorMin = new Vector2(0.5f, 0.52f);
        subTextRect.anchorMax = new Vector2(0.5f, 0.52f);
        subTextRect.pivot = new Vector2(0.5f, 0.5f);
        subTextRect.sizeDelta = new Vector2(600, 60);
        subTextRect.anchoredPosition = Vector2.zero;

        // ===== Main Garden button — matches Settings style =====
        CreateSpriteButton(panelGO.transform, "MainGardenButton", "Main Garden",
            BTN_GREEN_ROUND_ASSET, new Vector2(0, -120), new Vector2(450, 90), () =>
            {
                GoToMainGarden();
            });
    }

    private static void CreateSpriteButton(Transform parent, string name, string label,
        string spriteAssetPath, Vector2 position, Vector2 size,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = CreateUIElement(name, parent);

        // Image
        Image btnImage = btnGO.AddComponent<Image>();
        Sprite sprite = LoadSprite(spriteAssetPath);
        if (sprite != null)
        {
            btnImage.sprite = sprite;
            btnImage.type = Image.Type.Simple;
            btnImage.color = Color.white;
            btnImage.preserveAspect = false;
        }
        else
        {
            btnImage.color = new Color(0.2f, 0.65f, 0.35f);
            Debug.LogWarning($"[EndGameUI] Could not load sprite: {spriteAssetPath}");
        }
        btnImage.raycastTarget = true;

        // Button
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        btn.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f);
        colors.selectedColor = Color.white;
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = position;

        // Text label
        if (!string.IsNullOrEmpty(label))
        {
            GameObject btnTextGO = CreateUIElement("Text", btnGO.transform);
            Text btnText = btnTextGO.AddComponent<Text>();
            btnText.text = label;
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnText.fontSize = 30;
            btnText.fontStyle = FontStyle.Bold;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.horizontalOverflow = HorizontalWrapMode.Overflow;
            btnText.verticalOverflow = VerticalWrapMode.Overflow;
            btnText.color = Color.white;
            btnText.raycastTarget = false;

            Shadow shadow = btnTextGO.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);

            StretchToFill(btnTextGO.GetComponent<RectTransform>());
        }

        btn.onClick.AddListener(onClick);
    }

    /// <summary>
    /// Loads a sprite. Uses AssetDatabase in Editor, Resources.Load in builds.
    /// </summary>
    private static Sprite LoadSprite(string assetPath)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
#else
        // For builds: expects sprite in Resources folder
        // e.g. "Assets/UI/ButtonsText/ButtonText_Small_Green_Round.png"
        // -> Resources path: "UI/ButtonsText/ButtonText_Small_Green_Round"
        string resourcePath = assetPath
            .Replace("Assets/", "")
            .Replace(".png", "")
            .Replace(".jpg", "");
        return Resources.Load<Sprite>(resourcePath);
#endif
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

    private static void GoToMainGarden()
    {
        isShowing = false;
        if (canvasGO != null)
        {
            Object.Destroy(canvasGO);
            canvasGO = null;
        }
        Time.timeScale = 1f;
        MiniGameLauncher.ReturnToGarden();
    }
}
