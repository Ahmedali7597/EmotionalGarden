using UnityEngine;
using UnityEngine.EventSystems;

public class DragObjectMobile : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 startPos;
    private Camera cam;
    private SpriteRenderer sr;

    public string shapeType;
    public string colorType;

    public float checkRadius = 0.5f;

    private string[] shapes = new string[] { "Circle", "Square", "Triangle" };
    private Color[] colors = new Color[] { Color.red, Color.blue, Color.green };
    private string[] colorNames = new string[] { "Red", "Blue", "Green" };

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();

        // Only randomize if not assigned yet
        if (string.IsNullOrEmpty(shapeType) || string.IsNullOrEmpty(colorType))
        {
            RandomizeShapeAndColor();
        }
        else
        {
            ApplyColor();
        }
    }

    // New public method for ShapeManager to call
    public void RandomizeShapeAndColor()
    {
        int shapeIndex = Random.Range(0, shapes.Length);
        shapeType = shapes[shapeIndex];

        int colorIndex = Random.Range(0, colors.Length);
        colorType = colorNames[colorIndex];
        if (sr != null) sr.color = colors[colorIndex];
    }

    private void ApplyColor()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (colorType == "Red") sr.color = Color.red;
        else if (colorType == "Blue") sr.color = Color.blue;
        else if (colorType == "Green") sr.color = Color.green;
    }

    public void OnPointerDown(PointerEventData eventData) { }

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
                    socket.isFilled = true;
                    GameManager.instance.CheckWin();
                    return;
                }
            }
        }
        transform.position = startPos;
    }
}