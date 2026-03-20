using UnityEngine;
using UnityEngine.InputSystem;

public class Cloud : MonoBehaviour
{
    public float floatSpeed = 2f;      // The speed at which the cloud normally floats
    public float floatDistance = 1f;   // The distance the cloud moves left and right while floating
    public float swatPower = 30f;      // The force applied when swatted (higher value = flies further)
    public float friction = 3f;        // Friction applied to slow it down (higher value = stops faster)

    private Vector3 centerPosition;
    private Collider2D myCollider;
    private float timeOffset;

    // Variables related to the swipe/swat mechanic
    private Vector2 currentVelocity = Vector2.zero; // The current flying speed after being swatted
    private bool isSwatted = false;                 // Flag to check if the cloud is currently flying away
    private Vector2 previousTouchPosition;          // The position of the finger/mouse in the previous frame

    void Start()
    {
        centerPosition = transform.position;
        myCollider = GetComponent<Collider2D>();
        timeOffset = Random.Range(0f, Mathf.PI * 2);
    }

    void Update()
    {
        if (Pointer.current == null) return;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // 1. The first moment the screen is touched: initialize the previous position
        if (Pointer.current.press.wasPressedThisFrame)
        {
            previousTouchPosition = touchPosition;
        }

        // 2. While the finger is being dragged on the screen (Swiping)
        if (Pointer.current.press.isPressed)
        {
            // Calculate how far the input moved since the last frame (direction and speed)
            Vector2 swipeDelta = touchPosition - previousTouchPosition;

            // If the touch is inside the cloud's collider and actually moved swiftly
            if (myCollider.OverlapPoint(touchPosition) && swipeDelta.magnitude > 0.05f)
            {
                isSwatted = true;
                // Determine the velocity by multiplying the swipe direction by the swat power
                currentVelocity = swipeDelta * swatPower;
            }
            
            // Save the current position for the next frame's calculation
            previousTouchPosition = touchPosition;
        }

        // 3. Process the actual movement of the cloud
        if (isSwatted)
        {
            // Move the cloud based on its current velocity
            transform.position += (Vector3)currentVelocity * Time.deltaTime;
            
            // Apply friction to make it gradually slide to a stop
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, Time.deltaTime * friction);

            // If the velocity is close to 0, the cloud has practically stopped
            if (currentVelocity.magnitude < 0.1f)
            {
                isSwatted = false; // End the swatting state
                centerPosition = transform.position; // Set the stopped location as the new center point
                timeOffset = Time.time;
            }
        }
        else
        {
            // Normally, float left and right around the center position
            float newX = centerPosition.x + Mathf.Sin((Time.time - timeOffset) * floatSpeed) * floatDistance;
            transform.position = new Vector3(newX, centerPosition.y, transform.position.z);
        }
    }
}