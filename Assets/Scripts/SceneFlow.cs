using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central place for all scene transition calls.
/// Scene names must match the .unity file names (without extension).
/// </summary>
public static class SceneFlow
{
    public const string StartScene         = "StartScene";
    public const string AvatarSelectScene  = "AvatarSelectScene";
    public const string EmotionSelectScene = "EmotionSelectScene";
    public const string GardenScene        = "GardenScene";

    // Minigame scene names (must match .unity filenames)
    public static readonly string[] MiniGameScenes =
    {
        "CloudGame",
        "DarkGame",
        "PuzzleGame",
        "RainGame",
        "RunGame"
    };

    public static void GoToStart()         => SceneManager.LoadScene(StartScene);
    public static void GoToAvatarSelect()  => SceneManager.LoadScene(AvatarSelectScene);
    public static void GoToEmotionSelect() => SceneManager.LoadScene(EmotionSelectScene);
    public static void GoToGarden()        => SceneManager.LoadScene(GardenScene);

    /// <summary>Load a random minigame scene.</summary>
    public static void GoToRandomMiniGame()
    {
        int index = Random.Range(0, MiniGameScenes.Length);
        string scene = MiniGameScenes[index];
        Debug.Log($"[SceneFlow] Loading minigame: {scene}");
        SceneManager.LoadScene(scene);
    }

    /// <summary>Ensures GameData singleton exists.</summary>
    public static void EnsureGameData()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }
    }
}
