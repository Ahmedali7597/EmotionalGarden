using UnityEngine;
using UnityEngine.InputSystem;

public class Sun : MonoBehaviour
{
    public GameObject complete;

    // A global flag to track if the game is finished
    public static bool isCompleted = false;

    void Start()
    {
        // Always reset this to false when the game starts or restarts
        isCompleted = false;
    }

    void Update()
    {
        // Ignore input while Settings screen is open
        if (SettingsUI.isOpen) return;

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
                    if (!isCompleted)
                    {
                        isCompleted = true;

                        Instantiate(complete, Vector3.zero, Quaternion.identity);
                        Time.timeScale = 0f;

                        EndGameUI.Show(true);

                        Destroy(gameObject);
                    }
                    return;
                }
            }
        }
    }
}
