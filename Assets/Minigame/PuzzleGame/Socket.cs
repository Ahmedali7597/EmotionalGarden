using UnityEngine;

public class Socket : MonoBehaviour
{
    public string correctShape;
    public string correctColor;
    public bool isFilled = false;

    public SpriteRenderer sr;

    // Possible shapes and colors
    private string[] shapes = new string[] { "Circle", "Square", "Triangle" };
    private Color[] colors = new Color[] { Color.red, Color.blue, Color.green };
    private string[] colorNames = new string[] { "Red", "Blue", "Green" };

    public void RandomizeSocket()
    {
        int shapeIndex = Random.Range(0, shapes.Length);
        correctShape = shapes[shapeIndex];

        int colorIndex = Random.Range(0, colors.Length);
        correctColor = colorNames[colorIndex];
        if (sr != null)
            sr.color = colors[colorIndex];
    }
}