using UnityEngine;

/// <summary>
/// Modular minigame launcher.
/// Call LaunchRandom() from any script or UI button to load a random minigame.
/// After the minigame completes, the minigame scene should call
/// MiniGameLauncher.ReturnToGarden() to go back.
/// </summary>
public static class MiniGameLauncher
{
    /// <summary>Load a random minigame scene from the 5 available.</summary>
    public static void LaunchRandom()
    {
        Debug.Log("[MiniGameLauncher] Launching random minigame...");
        SceneFlow.GoToRandomMiniGame();
    }

    /// <summary>Return to the Garden scene after a minigame completes.</summary>
    public static void ReturnToGarden()
    {
        Debug.Log("[MiniGameLauncher] Returning to Garden...");
        SceneFlow.GoToGarden();
    }
}
