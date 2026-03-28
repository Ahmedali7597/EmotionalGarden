using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shared Settings button script for all minigames.
/// Attach to a GameObject with SpriteRenderer + BoxCollider2D.
/// Toggles SettingsUI on click.
/// </summary>
public class SettingsButton : MonoBehaviour
{
    void Update()
    {
        // Ignore if EndGame screen is showing
        if (EndGameUI.isShowing) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                SettingsUI.Toggle();
            }
        }
    }
}
