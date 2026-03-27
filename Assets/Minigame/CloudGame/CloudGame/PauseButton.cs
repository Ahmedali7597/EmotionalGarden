using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pauseScreen;

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                TogglePause();
            }
        }
    }

    private void TogglePause()
    {
        // FIX: If the game is already complete, ignore clicks on the pause button!
        if (Sun.isCompleted)
        {
            return; // Exit the function early so it doesn't unpause
        }

        isPaused = !isPaused; // Flip the pause state

        if (isPaused)
        {
            Time.timeScale = 0f; // Freeze time
            Instantiate(pauseScreen, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Time.timeScale = 1f; // Resume time
        }
    }
}