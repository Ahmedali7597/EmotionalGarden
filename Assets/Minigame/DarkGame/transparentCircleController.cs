using UnityEngine;
using UnityEngine.InputSystem; // New Input System

[RequireComponent(typeof(SpriteRenderer))]
public class TransparentCircleController2D : MonoBehaviour
{
    [Range(0f, 1f)]
    public float radius = 0.2f; // Circle size
    private Material mat;
    private Camera mainCam;
    private Vector2 uv = new Vector2(-1f, -1f); // Start off-screen
    private SpriteRenderer spriteRenderer;
    public Vector2 CurrentUV => uv;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mat = spriteRenderer.material;
        mat.SetFloat("_Radius", radius);
        mainCam = Camera.main;
    }

    void Update()
    {
        // Ignore input while Settings screen is open
        if (SettingsUI.isOpen) 
        {
            uv = new Vector2(-1f, -1f);
            mat.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
            return;
        }

        Vector2 screenPos = Vector2.zero;
        bool hasInput = false;

        // Touch input (mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            hasInput = true;
        }
        // Mouse input (editor/desktop)
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPos = Mouse.current.position.ReadValue();
            hasInput = true;
        }

        if (hasInput)
        {
            // Convert screen position to world position
            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCam.transform.position.z));

            // Convert world position to local position relative to sprite
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            // Convert local position to UV (0-1 range)
            Bounds bounds = spriteRenderer.sprite.bounds;
            float u = (localPos.x - bounds.min.x) / bounds.size.x;
            float v = (localPos.y - bounds.min.y) / bounds.size.y;


            uv = new Vector2(u, v);
            if (u < 0 || u > 1 || v < 0 || v > 1)
            {
                uv = new Vector2(-1f, -1f); // Hide
            }

        }
        else
        {
            uv = new Vector2(-1f, -1f); // Hide circle when no input
        }

        mat.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));

    }
    public Bounds GetSpriteBounds()
    {
        return spriteRenderer.sprite.bounds;
    }

}
