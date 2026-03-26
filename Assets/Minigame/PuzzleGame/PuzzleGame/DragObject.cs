using UnityEngine;
using UnityEngine.EventSystems; // Required for drag & drop functionality

public class DragObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Properties of the current shape (Set by ShapeManager)
    public string currentShape; 
    public string currentColor;

    private Vector2 offset; 
    private Camera mainCamera;
    private bool isDragging = false;
    private Socket currentSocket; // The socket it is currently placed in (null if none)

    private Rigidbody2D rb; // Reference to the Physics component

    void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D on Awake
    }

    // Function to set the data and color of the shape (Optimized for 2D SpriteRenderer)
    public void SetShapeAndColor(string shape, string colorName, Color colorValue)
    {
        currentShape = shape;
        currentColor = colorName;
        
        // Find SpriteRenderers in children and change the color directly
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        
        if (renderers != null && renderers.Length > 0)
        {
            foreach (SpriteRenderer sr in renderers)
            {
                // Set the color of the sprite (The most reliable way in 2D)
                sr.color = colorValue; 
            }
        }
        else
        {
            Debug.LogWarning($"Warning: DragObject [{gameObject.name}] has no SpriteRenderer! Please check the prefab.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Prevent dragging if the shape is already correctly placed in a socket
        if (currentSocket != null && currentSocket.isCorrect) return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        offset = (Vector2)transform.position - mousePos;
        isDragging = true;

        // Pause physics when grabbed
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
            transform.position = mousePos + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        // Snapping Logic
        Socket[] allSockets = GameObject.FindObjectsByType<Socket>(FindObjectsSortMode.None);
        float snapDistance = 1.0f; // Range to detect snapping
        Socket nearestSocket = null;
        float minDistance = float.MaxValue;

        // Search for nearby sockets
        foreach (Socket socket in allSockets)
        {
            float dist = Vector2.Distance(transform.position, socket.transform.position);
            if (dist < snapDistance && dist < minDistance)
            {
                minDistance = dist;
                nearestSocket = socket;
            }
        }

        // 1. When dropped near a socket
        if (nearestSocket != null)
        {
            // Ask the socket to verify if this is the correct match
            bool matched = nearestSocket.CheckPlacedObject(this); 

            if (matched)
            {
                // KEY FIX for Physics & Animation: Snap perfectly to the socket
                currentSocket = nearestSocket;
                
                // 1. Align position perfectly
                transform.position = currentSocket.transform.position; 
                
                // 2. Reset rotation (Ensures animations play in the correct orientation)
                transform.rotation = Quaternion.identity; 

                // 3. Lock the Physics Engine completely
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero; // Set velocity to 0
                    rb.angularVelocity = 0f;      // Set angular velocity to 0
                    rb.isKinematic = true;        // Change to Kinematic mode (Stops gravity/collisions)
                }

                // Disable the collider (Prevent further movement)
                Collider2D col = GetComponent<Collider2D>();
                if(col != null) col.enabled = false; 

                Debug.Log($"[{currentShape}] snapped to Socket with rotation reset.");

                // Trigger Win Check in GameManager
                if (GameManager.instance != null) { GameManager.instance.CheckWin(); }
            }
            else
            {
                // If incorrect: Object stays where it is or falls (Gravity takes over if rb.isKinematic is false)
                currentSocket = null;
            }
        }
        else
        {
            // Not near any socket
            currentSocket = null;
        }
    }
}