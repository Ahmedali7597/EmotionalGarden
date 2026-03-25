using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject creaturePrefab;       // The sprite with ReactToLight + Animator
    public Transform[] targetPoints;        // Assign 2 (or more) target points
    public int creatureCount = 2;           // How many to spawn
    public Vector2 spawnAreaMin;            // Bottom-left of spawn area
    public Vector2 spawnAreaMax;            // Top-right of spawn area
    public TransparentCircleController2D lightController;
    public RuneGameManager gameManager;


    void Start()
    {
        for (int i = 0; i < creatureCount; i++)
        {
            // Pick a random spawn position
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            Vector3 spawnPos = new Vector3(x, y, 0);

            // Spawn the creature
            GameObject creature = Instantiate(creaturePrefab, spawnPos, Quaternion.identity);

            // Assign the light controller
            var react = creature.GetComponent<ReactToLight>();
            react.lightController = lightController;

            // Assign a unique target point
            react.targetPoint = targetPoints[i];
            react.gameManager = gameManager;

        }
    }
}
