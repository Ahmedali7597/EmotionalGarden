using UnityEngine;

public class SlimeObstacle : MonoBehaviour
{
    public float speed = 5.0f; // Speed at which the slime moves
    public float deadZone = -10.0f; // X coordinate where the slime gets destroyed (off-screen left)

    void Update()
    {
        // 1. Continuously move to the left.
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. If the slime's X position goes further left than the deadZone
        if (transform.position.x < deadZone)
        {
            // Destroy this slime object to save memory.
            Destroy(gameObject); 
        }
    }
}