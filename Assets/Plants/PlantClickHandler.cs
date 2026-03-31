using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Place on a single manager GameObject (e.g. GardenManager).
/// Detects clicks/taps via the new Input System and 2D physics raycast.
/// When the player clicks/taps a plant, it grows only that plant.
/// </summary>
public class PlantClickHandler : MonoBehaviour
{
    private Camera mainCam;
    private Mouse mouse;
    private Touchscreen touchscreen;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("[PlantClickHandler] No Main Camera found in scene.");
        }

        mouse = Mouse.current;
        touchscreen = Touchscreen.current;
    }

    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        bool clicked = false;
        Vector2 screenPos = Vector2.zero;

        // Check mouse click
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            clicked = true;
            screenPos = mouse.position.ReadValue();
        }
        // Check touch input
        else if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
        {
            clicked = true;
            screenPos = touchscreen.primaryTouch.position.ReadValue();
        }

        if (!clicked)
            return;

        // Cast a ray from the camera through the click/tap position
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if a plant was clicked
            Plant plant = hit.collider.GetComponent<Plant>();
            if (plant != null)
            {
                Debug.Log($"[PlantClickHandler] {plant.gameObject.name} clicked — growing this plant.");
                plant.Grow();
                return;
            }

            // Check if a rune was clicked
            RuneClickable rune = hit.collider.GetComponent<RuneClickable>();
            if (rune != null)
            {
                rune.OnClicked();
                return;
            }
        }
    }
}
