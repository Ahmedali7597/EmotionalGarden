using UnityEngine;

public class BlackScreenReveal : MonoBehaviour
{
    private Material mat;
    public float revealSpeed = 0.5f;
    private bool revealing = false;
    private float radius = 0f;
    void Start()
    {
        // Create a unique material instance for THIS black screen
        mat = GetComponent<SpriteRenderer>().material;

        // Reset radius
        mat.SetFloat("_RevealRadius", 0f);
        radius = 0f;
    }


    void Update()
    {
        if (!revealing) return;

        radius += revealSpeed * Time.deltaTime * 2f;
        mat.SetFloat("_RevealRadius", radius);

        if (radius > 2f)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartReveal()
    {
        revealing = true;
    }
    public void ResetReveal()
    {
        radius = 0f;
        mat.SetFloat("_RevealRadius", 0f);
    }
}
