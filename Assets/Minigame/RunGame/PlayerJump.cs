using UnityEngine;
using UnityEngine.InputSystem; // Using the new Input System!

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 5.0f; // Force applied when jumping
    private Rigidbody2D rb; // Reference to the physics component
    private bool isGrounded = true; // Checks if the player is on the ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        
        // Ensure time flows normally when the game starts (prevents pausing on restart)
        Time.timeScale = 1f; 
    }

    void Update()
    {
        bool isJumpPressed = false;

        // 1. Detect left mouse button click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isJumpPressed = true;
        }
        
        // 2. Detect mobile screen touch (for app games)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isJumpPressed = true;
        }

        // Jump if the button was pressed and the player is on the ground!
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; 
        }
    }

    // Collision detection function
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. When colliding with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }
        // 2. When colliding with an obstacle (slime)
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameOver(); 
        }
    }

    // Game over handling function
    private void GameOver()
    {
        Debug.Log("Game Over!"); // Print a message to the console

        // Stop everything by setting the game time to 0!
        Time.timeScale = 0f; 
    }
}