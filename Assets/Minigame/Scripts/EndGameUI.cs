using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Utility class that programmatically creates a shared EndGame screen.
/// Call EndGameUI.Show(true/false) from any scene.
/// </summary>
public static class EndGameUI
{
    private static GameObject canvasGO;
    public static bool isShowing = false;

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

    private static void CreateUI(bool success)
    {
        // ===== Canvas =====
        canvasGO = new GameObject("EndGameCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== EventSystem check =====
        EnsureEventSystem();

        // ===== Semi-transparent background =====
        GameObject bgGO = CreateUIElement("Background", canvasGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        StretchToFill(bgRect);

        // ===== Center panel =====
        GameObject panelGO = CreateUIElement("Panel", canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700, 500);
        panelRect.anchoredPosition = Vector2.zero;

        // ===== Result text (Success / Fail) =====
        GameObject textGO = CreateUIElement("ResultText", panelGO.transform);
        Text resultText = textGO.AddComponent<Text>();
        resultText.text = success ? "Success!" : "Fail...";
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resultText.fontSize = 90;
        resultText.fontStyle = FontStyle.Bold;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.horizontalOverflow = HorizontalWrapMode.Overflow;
        resultText.verticalOverflow = VerticalWrapMode.Overflow;
        resultText.color = success ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.65f);
        textRect.anchorMax = new Vector2(0.5f, 0.65f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(600, 150);
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

        RectTransform subTextRect = subTextGO.GetComponent<RectTransform>();
        subTextRect.anchorMin = new Vector2(0.5f, 0.48f);
        subTextRect.anchorMax = new Vector2(0.5f, 0.48f);
        subTextRect.pivot = new Vector2(0.5f, 0.5f);
        subTextRect.sizeDelta = new Vector2(600, 60);
        subTextRect.anchoredPosition = Vector2.zero;

        // ===== Main Garden button =====
        CreateButton(panelGO.transform, "MainGardenButton", "Main Garden",
            new Color(0.2f, 0.65f, 0.35f), new Vector2(0, -100), () =>
            {
                GoToMainGarden();
            });
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
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(400, 90);
        btnRect.anchoredPosition = position;

        // Button text
        GameObject btnTextGO = CreateUIElement("Text", btnGO.transform);
        Text btnText = btnTextGO.AddComponent<Text>();
        btnText.text = label;
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 42;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        RectTransform btnTextRect = btnTextGO.GetComponent<RectTransform>();
        StretchToFill(btnTextRect);

        btn.onClick.AddListener(onClick);
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
        canvasGO = null;
        MiniGameLauncher.ReturnToGarden();
    }
}
