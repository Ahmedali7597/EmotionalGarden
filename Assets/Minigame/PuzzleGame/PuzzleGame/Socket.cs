using UnityEngine;

public class Socket : MonoBehaviour
{
    [Header("Unique Shape of this Socket")]
    // Enter "Rune1", "Rune2", or "Rune3" here in the Inspector.
    public string myShapeName; 

    public string correctShape; 
    public string correctColor;
    public bool isCorrect = false; 

    [Header("Graphics")]
    public SpriteRenderer targetSpriteRenderer; 
    public SpriteRenderer glowRenderer; 
    public Color glowColor = Color.green;

    void Start()
    {
        SetSolvedState(false);
    }

    public void SetSolution(string colorName, Color colorValue)
    {
        // The socket uses its own myShapeName as the required answer.
        correctShape = myShapeName; 
        correctColor = colorName;
        
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = colorValue;
        }
        else
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = colorValue;
        }
    }

    public bool CheckPlacedObject(DragObject placedObject)
    {
        if (placedObject == null || string.IsNullOrEmpty(placedObject.currentShape)) return false;

        bool shapeMatches = placedObject.currentShape == correctShape;
        bool colorMatches = placedObject.currentColor == correctColor;

        if (shapeMatches && colorMatches)
        {
            Debug.Log($"Match Success: Correct! [{correctShape}/{correctColor}]");
            SetSolvedState(true);
            return true;
        }
        else
        {
            Debug.Log($"Match Fail: Incorrect. Needs [{correctShape}/{correctColor}], got [{placedObject.currentShape}/{placedObject.currentColor}]");
            return false;
        }
    }

    void SetSolvedState(bool solved)
    {
        isCorrect = solved;
        if (glowRenderer != null)
        {
            glowRenderer.color = solved ? glowColor : Color.white;
            glowRenderer.gameObject.SetActive(solved);
        }
    }
}