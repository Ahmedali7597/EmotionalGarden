using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Place on a single manager GameObject (e.g. GardenManager).
/// Detects clicks/taps via the new Input System and 2D physics overlap.
/// When the player clicks/taps a plant, it grows only that plant.
/// Uses Pointer.current which handles both mouse and touch input.
/// 
/// Includes cooldown to prevent rapid-fire clicks from causing issues.
/// </summary>
public class PlantClickHandler : MonoBehaviour
{
    private Camera mainCam;

    // Cooldown to prevent rapid-fire clicks from causing state issues
    private float lastClickRealTime = -1f;
    private const float CLICK_COOLDOWN = 0.15f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        // Don't process if settings UI is open
        if (SettingsUI.isOpen) return;

        // Don't process if end game screen is showing
        if (EndGameUI.isShowing) return;

        // Cooldown check (use unscaled time since timeScale may be 0)
        if (Time.realtimeSinceStartup - lastClickRealTime < CLICK_COOLDOWN) return;

        // Use Pointer.current — works for both mouse and touch
        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame)
            return;

        // Don't process if clicking on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 screenPos = pointer.position.ReadValue();
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(screenPos);

        // Use OverlapPointAll — reliable point-based 2D hit detection
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            // Check if a plant was clicked
            Plant plant = hits[i].GetComponent<Plant>();
            if (plant != null)
            {
                lastClickRealTime = Time.realtimeSinceStartup;
                Debug.Log($"[PlantClickHandler] {plant.gameObject.name} clicked — growing this plant.");
                plant.Grow();
                return;
            }

            // Check if a rune was clicked
            RuneClickable rune = hits[i].GetComponent<RuneClickable>();
            if (rune != null)
            {
                lastClickRealTime = Time.realtimeSinceStartup;
                rune.OnClicked();
                return;
            }
        }
    }
}
