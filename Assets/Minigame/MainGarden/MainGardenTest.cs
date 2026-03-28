using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// MainGarden test scene script.
/// Displays a single button to launch a random minigame.
/// </summary>
public class MainGardenTest : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        CreateUI();
    }

    void CreateUI()
    {
        // Canvas
        GameObject canvasGO = new GameObject("MainGardenCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.35f, 0.65f, 0.35f, 1f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title text
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvasGO.transform, false);
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Emotion Garden";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 80;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(800, 120);

        // Description text
        GameObject descGO = new GameObject("Description");
        descGO.transform.SetParent(canvasGO.transform, false);
        Text descText = descGO.AddComponent<Text>();
        descText.text = "Tap to play a random minigame!";
        descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.fontSize = 36;
        descText.alignment = TextAnchor.MiddleCenter;
        descText.color = new Color(1f, 1f, 1f, 0.7f);
        descText.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.58f);
        descRect.anchorMax = new Vector2(0.5f, 0.58f);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.sizeDelta = new Vector2(800, 60);

        // Random play button
        GameObject btnGO = new GameObject("PlayButton");
        btnGO.transform.SetParent(canvasGO.transform, false);

        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.95f, 0.65f, 0.15f, 1f); // Orange

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(1f, 0.75f, 0.3f);
        colors.pressedColor = new Color(0.8f, 0.5f, 0.1f);
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.38f);
        btnRect.anchorMax = new Vector2(0.5f, 0.38f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(450, 100);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);
        Text btnText = textGO.AddComponent<Text>();
        btnText.text = "Play!";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 50;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        btn.onClick.AddListener(() =>
        {
            MiniGameLauncher.LaunchRandom();
        });
    }
}
