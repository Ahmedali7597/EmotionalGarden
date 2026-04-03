using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shared Settings button script for all scenes.
/// Attach to a GameObject with SpriteRenderer + BoxCollider2D.
/// Toggles SettingsUI on click/tap.
/// Uses Pointer.current which handles both mouse and touch input.
/// Uses OverlapPointAll for reliable 2D point detection.
/// </summary>
public class SettingsButton : MonoBehaviour
{
    void Update()
    {
        // Ignore if EndGame screen is showing
        if (EndGameUI.isShowing) return;

        // Ignore if settings is already open
        if (SettingsUI.isOpen) return;

        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame) return;

        if (Camera.main == null) return;

        Vector2 screenPosition = pointer.position.ReadValue();
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Use OverlapPointAll — reliable point-based 2D hit detection
        Collider2D[] hits = Physics2D.OverlapPointAll(touchPosition);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].gameObject == gameObject)
            {
                SettingsUI.Toggle();
                return;
            }
        }
    }
}
