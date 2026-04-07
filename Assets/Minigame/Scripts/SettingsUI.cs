using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Manages a pre-built SettingsCanvas that exists in the scene hierarchy (hidden by default).
/// Call SettingsUI.Toggle() from any scene — it finds the canvas by name and shows/hides it.
/// Sound values are saved to PlayerPrefs for later use with AudioSource.
/// 
/// The canvas should be a child-disabled GameObject named "SettingsCanvas" in the scene.
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
        if (isOpen)
        {
            // Safety: if isOpen but canvas is gone, reset state
            if (canvasGO == null)
            {
                isOpen = false;
                Time.timeScale = 1f;
            }
            else
            {
                return;
            }
        }
        isOpen = true;

        // Load saved volume from PlayerPrefs
        gameVolume = PlayerPrefs.GetFloat(GAME_VOLUME_KEY, 1f);
        bgVolume = PlayerPrefs.GetFloat(BG_VOLUME_KEY, 1f);

        Time.timeScale = 0f;

        // Always re-find the canvas (it may have been lost on scene change)
        canvasGO = FindSettingsCanvas();

        if (canvasGO != null)
        {
            // Activate the canvas — this triggers OnEnable in SettingsCanvasWiring
            // which wires up listeners and handles layout
            canvasGO.SetActive(true);

            // Sync slider values to current saved volumes
            SyncSliders();
        }
        else
        {
            Debug.LogWarning("[SettingsUI] No SettingsCanvas found in scene. Please add one.");
        }
    }

    public static void Hide()
    {
        // Always reset state even if not technically open (safety for stuck states)
        isOpen = false;

        // Save volume values
        PlayerPrefs.SetFloat(GAME_VOLUME_KEY, gameVolume);
        PlayerPrefs.SetFloat(BG_VOLUME_KEY, bgVolume);
        PlayerPrefs.Save();

        if (canvasGO != null)
        {
            canvasGO.SetActive(false);
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

    /// <summary>
    /// Called when the game sound slider value changes.
    /// </summary>
    public static void OnGameVolumeChanged(float value)
    {
        gameVolume = value;
        PlayerPrefs.SetFloat(GAME_VOLUME_KEY, value);
        ApplyVolumeToActiveSources();
    }

    /// <summary>
    /// Called when the background sound slider value changes.
    /// </summary>
    public static void OnBGVolumeChanged(float value)
    {
        bgVolume = value;
        PlayerPrefs.SetFloat(BG_VOLUME_KEY, value);
        ApplyVolumeToActiveSources();
    }

    /// <summary>
    /// Called by the Reselect Emotion button.
    /// </summary>
    public static void OnReselectEmotion()
    {
        isOpen = false;
        SaveAndCleanup();
        Time.timeScale = 1f;
        SceneFlow.GoToEmotionSelect();
    }

    /// <summary>
    /// Called by the Main Garden button.
    /// </summary>
    public static void OnMainGarden()
    {
        isOpen = false;
        SaveAndCleanup();
        Time.timeScale = 1f;
        MiniGameLauncher.ReturnToGarden();
    }

    /// <summary>
    /// Called by the Resume button.
    /// </summary>
    public static void OnResume()
    {
        Hide();
    }

    /// <summary>
    /// Resets the cached canvas reference. Call this on scene load.
    /// </summary>
    public static void ResetCanvasReference()
    {
        canvasGO = null;
        isOpen = false;
    }

    private static void SaveAndCleanup()
    {
        PlayerPrefs.SetFloat(GAME_VOLUME_KEY, gameVolume);
        PlayerPrefs.SetFloat(BG_VOLUME_KEY, bgVolume);
        PlayerPrefs.Save();
        if (canvasGO != null)
        {
            canvasGO.SetActive(false);
        }
        canvasGO = null;
    }

    private static GameObject FindSettingsCanvas()
    {
        // Search for inactive objects too
        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (var canvas in allCanvases)
        {
            if (canvas.gameObject.name == "SettingsCanvas" && canvas.gameObject.scene.isLoaded)
            {
                return canvas.gameObject;
            }
        }
        return null;
    }

    private static void SyncSliders()
    {
        if (canvasGO == null) return;

        // Find sliders by name and set their values
        Slider[] sliders = canvasGO.GetComponentsInChildren<Slider>(true);
        foreach (var slider in sliders)
        {
            if (slider.gameObject.name == "GameSoundSlider")
            {
                slider.SetValueWithoutNotify(gameVolume);
            }
            else if (slider.gameObject.name == "BGSoundSlider")
            {
                slider.SetValueWithoutNotify(bgVolume);
            }
        }
    }
}
