using System.Collections;
using UnityEngine;

public class ReactToLight : MonoBehaviour
{
    public TransparentCircleController2D lightController;
    public Transform targetPoint;
    public float moveSpeed = 2f;

    private bool activated = false;
    private bool animationPlayed = false; // NEW
    private Animator anim;
    public RuneGameManager gameManager;
    public float activationDelay = 0.5f;
    private bool waitingToActivate = false;

    void Start()
    {
        anim = GetComponent<Animator>(); // Get animator on this object
    }

    void Update()
    {
        // If already activated, keep moving
        if (activated)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint.position,
                moveSpeed * Time.deltaTime
            );

            // Check if we reached the target
            if (!animationPlayed &&
                Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
            {
                animationPlayed = true;
                anim.SetTrigger("ReachedTarget"); // Play animation
                // Notify the game manager
                gameManager.RuneCompleted();
            }

            return;
        }

        // Otherwise, check if the light touches it
        Vector2 uv = lightController.CurrentUV;
        float radius = lightController.radius;

        if (uv.x < 0 || uv.y < 0)
            return;

        Vector3 localPos = lightController.transform.InverseTransformPoint(transform.position);

        Bounds b = lightController.GetSpriteBounds();
        float u = (localPos.x - b.min.x) / b.size.x;
        float v = (localPos.y - b.min.y) / b.size.y;

        float dist = Vector2.Distance(new Vector2(u, v), uv);

        if (dist < radius && !waitingToActivate)
        {
            StartCoroutine(ActivateAfterDelay());
        }

    }
    IEnumerator ActivateAfterDelay()
    {
        waitingToActivate = true;
        yield return new WaitForSeconds(activationDelay);
        activated = true;
    }

}
