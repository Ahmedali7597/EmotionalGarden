using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Automatically applies the saved volume settings to all AudioSources
/// whenever a new scene is loaded.
/// </summary>
public static class AudioVolumeManager
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyVolume();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolume();
    }

    private static void ApplyVolume()
    {
        SettingsUI.ApplyVolumeToActiveSources();
    }
}
