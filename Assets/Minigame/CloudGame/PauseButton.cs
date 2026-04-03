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
            if (Camera.main == null) return;

            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // Use OverlapPointAll for reliable 2D point detection
            Collider2D[] hits = Physics2D.OverlapPointAll(touchPosition);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null && hits[i].gameObject == gameObject)
                {
                    TogglePause();
                    return;
                }
            }
        }
    }

    private void TogglePause()
    {
        // If the game is already complete, ignore clicks on the pause button
        if (Sun.isCompleted)
        {
            return;
        }

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            Instantiate(pauseScreen, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
