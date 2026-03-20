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
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // Ensure we only trigger the complete sequence once
                if (!isCompleted) 
                {
                    isCompleted = true; // Mark the game as complete
                    
                    Instantiate(complete, Vector3.zero, Quaternion.identity);
                    Time.timeScale = 0f; // Freeze time
                    
                    Destroy(gameObject); 
                }
            }
        }
    }
}