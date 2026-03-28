using UnityEngine;

/// <summary>
/// Attach this to any GameObject in a minigame scene.
/// Call Complete() when the minigame finishes to return the player to the Garden.
/// 
/// Usage from any minigame script:
///   FindFirstObjectByType&lt;MiniGameComplete&gt;().Complete();
/// Or simply:
///   MiniGameLauncher.ReturnToGarden();
/// </summary>
public class MiniGameComplete : MonoBehaviour
{
    /// <summary>
    /// Call this when the minigame is finished.
    /// Returns the player to the Garden scene.
    /// </summary>
    public void Complete()
    {
        Debug.Log("[MiniGameComplete] Minigame finished — returning to Garden.");
        MiniGameLauncher.ReturnToGarden();
    }
}