using UnityEngine;

public class Socket : MonoBehaviour
{
    public string correctShape; // Assigned in the Inspector
    public string correctColor; // Now dynamically set by ShapeManager
    public bool isFilled = false;
    public SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // 👈 ShapeManager will pick a unique color and pass it to this function!
    public void SetColor(string colorName, Color actualColor)
    {
        correctColor = colorName;
        if(sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = actualColor;
    }
}