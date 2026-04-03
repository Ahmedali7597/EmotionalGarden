using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
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
                    Destroy(gameObject);
                    Time.timeScale = 1f;
                    return;
                }
            }
        }
    }
}
