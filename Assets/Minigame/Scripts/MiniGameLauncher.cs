using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Modular minigame launcher.
/// Call LaunchRandom() from any script or UI button to load a random minigame.
/// After the minigame completes, the minigame scene should call
/// MiniGameLauncher.ReturnToGarden() to go back.
/// </summary>
public static class MiniGameLauncher
{
    private static readonly string[] miniGameScenes = new string[]
    {
        "CloudGame",
        "PuzzleGame",
        "RunGame",
        "DarkGame"
    };

    /// <summary>Load a random minigame scene from the 4 available.</summary>
    public static void LaunchRandom()
    {
        int index = Random.Range(0, miniGameScenes.Length);
        string sceneName = miniGameScenes[index];
        Debug.Log($"[MiniGameLauncher] Launching: {sceneName}");
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>Return to the Garden scene after a minigame completes.</summary>
    public static void ReturnToGarden()
    {
        Debug.Log("[MiniGameLauncher] Returning to Garden...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGarden");
    }
}
