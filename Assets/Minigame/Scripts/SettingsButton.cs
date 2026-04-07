using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Shared Settings button script for all scenes.
/// Attach to a GameObject with SpriteRenderer + BoxCollider2D.
/// Toggles SettingsUI on click/tap.
/// Uses Pointer.current which handles both mouse and touch input.
/// Uses OverlapPointAll for reliable 2D point detection.
/// 
/// Includes cooldown to prevent rapid-fire toggling from fast clicks.
/// </summary>
public class SettingsButton : MonoBehaviour
{
    // Cooldown to prevent double-toggle from fast clicks (in real time, ignores timeScale)
    private float lastToggleRealTime = -1f;
    private const float TOGGLE_COOLDOWN = 0.3f;

    void Update()
    {
        // Ignore if EndGame screen is showing
        if (EndGameUI.isShowing) return;

        // Ignore if settings is already open
        if (SettingsUI.isOpen) return;

        // Cooldown check (use unscaled time since timeScale may be 0)
        if (Time.realtimeSinceStartup - lastToggleRealTime < TOGGLE_COOLDOWN) return;

        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame) return;

        if (Camera.main == null) return;

        // Skip if pointer is over UI (prevents conflict with UI buttons)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 screenPosition = pointer.position.ReadValue();
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Use OverlapPointAll — reliable point-based 2D hit detection
        Collider2D[] hits = Physics2D.OverlapPointAll(touchPosition);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].gameObject == gameObject)
            {
                lastToggleRealTime = Time.realtimeSinceStartup;
                SettingsUI.Toggle();
                return;
            }
        }
    }
}
