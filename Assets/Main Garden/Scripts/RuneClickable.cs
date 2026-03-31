using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Marker component added to spawned slimes in the garden.
/// When detected by the PlantClickHandler raycast, it launches a random minigame.
/// </summary>
public class RuneClickable : MonoBehaviour
{
    /// <summary>
    /// Called by PlantClickHandler when the player clicks on this slime.
    /// Launches a random minigame.
    /// </summary>
    public void OnClicked()
    {
        Debug.Log($"[RuneClickable] {gameObject.name} clicked — launching random minigame!");
        SceneManager.LoadScene("MainGarden");
    }
}
