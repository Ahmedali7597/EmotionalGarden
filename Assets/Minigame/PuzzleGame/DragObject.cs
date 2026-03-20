using UnityEngine;
using UnityEngine.EventSystems;

public class DragObjectMobile : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 startPos;
    private Camera cam;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public string shapeType;
    public string colorType;
    public float checkRadius = 0.5f;

    private string[] shapes = new string[] { "Circle", "Square", "Triangle" };
    
    // 💡 Matched the count and order to exactly 9 to prevent OutOfBounds errors!
    private Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan, Color.gray, Color.white, Color.black };
    private string[] colorNames = new string[] { "Red", "Blue", "Green", "Yellow", "Magenta", "Cyan", "Gray", "White", "Black" };

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); 
        startPos = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData) 
    { 
        // Switch to Kinematic the moment it's grabbed to ignore gravity
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 touchPos = cam.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(touchPos.x, touchPos.y, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        foreach (Collider2D hit in hits)
        {
            Socket socket = hit.GetComponent<Socket>();
            if (socket != null && !socket.isFilled)
            {
                if (socket.correctShape == shapeType && socket.correctColor == colorType)
                {
                    transform.position = socket.transform.position;
                    transform.rotation = Quaternion.identity;
                    socket.isFilled = true;
                     GameManager.instance.CheckWin();

                    // Disable physics since it snapped into the correct socket (Keep Kinematic)
                    if (rb != null) 
                    {
                        rb.bodyType = RigidbodyType2D.Kinematic; 
                        rb.linearVelocity = Vector2.zero;
                    }
                    
                    return; // Exit the function
                }
            }
        }
        
        // Turn gravity back on so it falls to the floor if incorrect (Dynamic)
        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.linearVelocity = Vector2.zero; 
        }
    }
    
    public void SetShapeAndColor(string shape, string color)
    {
        shapeType = shape;
        colorType = color;

        // Apply the corresponding color from the arrays
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i] == color)
            {
                sr.color = colors[i];
                break;
            }
        }
    }
}