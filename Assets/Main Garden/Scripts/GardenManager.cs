using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main Garden scene controller.
/// - Spawns avatar from GameData
/// - Sets placeholder background color based on emotion (to be replaced later with real backgrounds)
/// - Manages minigame UI button (top-right, appears on random timer)
/// - Settings button to reselect emotion (NOT avatar)
/// </summary>
public class GardenManager : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Camera mainCamera;

    [Header("Minigame UI")]
    [SerializeField] private Button minigameButton;
    [SerializeField] private float minigameTimerMin = 30f;
    [SerializeField] private float minigameTimerMax = 60f;

    [Header("Settings UI")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button emotionSadButton;
    [SerializeField] private Button emotionCalmButton;
    [SerializeField] private Button emotionEnergeticButton;
    [SerializeField] private Button emotionAnxiousButton;
    [SerializeField] private Button closeSettingsButton;

    private Coroutine _minigameTimerCoroutine;

    private void Awake()
    {
        SceneFlow.EnsureGameData();
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // ── Self-wire and spawn avatar ────────────────────────────────
        var spawner    = FindFirstObjectByType<AvatarSpawner>();
        var movement   = FindFirstObjectByType<AvatarMovementController>();
        var wander     = FindFirstObjectByType<AvatarRandomWander>();
        var boundaryGO = GameObject.Find("Boundary");
        Collider2D bounds = boundaryGO != null ? boundaryGO.GetComponent<Collider2D>() : null;

        if (spawner != null)
        {
            var avatar = spawner.SpawnFromGameData();
            if (avatar != null)
            {
                avatar.SetGardenBounds(bounds);
                if (movement != null)
                {
                    movement.SetBoundary(bounds);
                    movement.SetWander(wander);
                    movement.SetAvatar(avatar);
                }
                wander?.SetAvatar(avatar);
            }
        }

        // ── Background ───────────────────────────────────────────────
        ApplyEmotionBackground();

        // ── Minigame button — hidden, appears on timer ───────────────
        if (minigameButton != null)
        {
            minigameButton.gameObject.SetActive(false);
            minigameButton.onClick.AddListener(OnMinigameButtonClicked);
        }
        StartMinigameTimer();

        // ── Settings ─────────────────────────────────────────────────
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(OnCloseSettings);
        if (emotionSadButton != null)
            emotionSadButton.onClick.AddListener(() => OnEmotionReselected(GameData.Emotion.Sad));
        if (emotionCalmButton != null)
            emotionCalmButton.onClick.AddListener(() => OnEmotionReselected(GameData.Emotion.Calm));
        if (emotionEnergeticButton != null)
            emotionEnergeticButton.onClick.AddListener(() => OnEmotionReselected(GameData.Emotion.Energetic));
        if (emotionAnxiousButton != null)
            emotionAnxiousButton.onClick.AddListener(() => OnEmotionReselected(GameData.Emotion.Anxious));

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // ── Background (placeholder — uses camera background color) ───────
    private void ApplyEmotionBackground()
    {
        if (mainCamera == null || GameData.Instance == null) return;

        // Placeholder colors — replace with real background sprites/images later
        mainCamera.backgroundColor = GameData.Instance.SelectedEmotion switch
        {
            GameData.Emotion.Sad       => new Color(0.25f, 0.30f, 0.45f), // muted blue
            GameData.Emotion.Calm      => new Color(0.30f, 0.55f, 0.35f), // soft green
            GameData.Emotion.Energetic => new Color(0.60f, 0.50f, 0.20f), // warm gold
            GameData.Emotion.Anxious   => new Color(0.50f, 0.25f, 0.30f), // muted red
            _                          => new Color(0.30f, 0.55f, 0.35f)
        };
    }

    // ── Minigame Timer ────────────────────────────────────────────────
    private void StartMinigameTimer()
    {
        if (_minigameTimerCoroutine != null)
            StopCoroutine(_minigameTimerCoroutine);
        _minigameTimerCoroutine = StartCoroutine(MinigameTimerRoutine());
    }

    private IEnumerator MinigameTimerRoutine()
    {
        float delay = Random.Range(minigameTimerMin, minigameTimerMax);
        yield return new WaitForSeconds(delay);

        if (minigameButton != null)
            minigameButton.gameObject.SetActive(true);

        _minigameTimerCoroutine = null;
    }

    private void OnMinigameButtonClicked()
    {
        MiniGameLauncher.LaunchRandom();
    }

    // ── Settings / Emotion Reselect ───────────────────────────────────
    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void OnCloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnEmotionReselected(GameData.Emotion emotion)
    {
        GameData.Instance.SelectedEmotion = emotion;
        ApplyEmotionBackground();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Debug.Log($"[GardenManager] Emotion changed to {emotion}. Background updated.");
    }
}
