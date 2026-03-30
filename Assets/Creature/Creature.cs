using UnityEngine;

public class Creature : MonoBehaviour
{
    public float speed = 2.0f; 
    public Garden garden; 

    // --- Movement Variables ---
    private Vector2 moveDirection; 
    private float changeDirectionTimer; 
    public float timeToChangeDirection = 2.0f; 

    [Header("Screen Boundaries")]
    public float minX; // Left boundary
    public float maxX; // Right boundary
    public float minY; // Bottom boundary
    public float maxY; // Top boundary

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        if (gameObject.activeSelf) 
        {
            Move();
        }
    }

    public void Move()
    {
        changeDirectionTimer -= Time.deltaTime;

        if (changeDirectionTimer <= 0)
        {
            ChooseNewDirection(); 
        }

        // 1. Move the slime
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // 2. Clamp the position to keep it inside the screen
        // Mathf.Clamp(current value, minimum value, maximum value)
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        
        // Apply the clamped position back to the slime
        transform.position = clampedPosition;
    }

    private void ChooseNewDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        moveDirection = new Vector2(randomX, randomY).normalized; 
        changeDirectionTimer = timeToChangeDirection; 
    }

    public void CheckGardenEmotion()
    {
        if (garden != null && garden.emotion == "Energetic") 
        {
            gameObject.SetActive(true); 
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}