using UnityEngine;
using System.Collections.Generic; // 👈 Required to use List (pool) functionality!

public class ShapeManager : MonoBehaviour
{
    public Socket[] sockets;
    public GameObject[] shapePrefabs;
    public int shapesPerSocket = 3;

    private string[] shapeNames = { "Circle", "Square", "Triangle" };
    
    // Using the same 9-color set used in DragObjectMobile!
    private Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan, Color.gray, Color.white, Color.black };
    private string[] colorNames = new string[] { "Red", "Blue", "Green", "Yellow", "Magenta", "Cyan", "Gray", "White", "Black" };

    void Start()
    {
        // 1️⃣ Create a List (pool) to hold color indices (0~8).
        List<int> availableColorPool = new List<int>();
        for (int i = 0; i < colorNames.Length; i++)
        {
            availableColorPool.Add(i);
        }

        foreach (Socket socket in sockets)
        {
            // 2️⃣ Pick a unique color for the socket and remove it from the pool!
            int socketColorIdx = GetUniqueRandomIndex(availableColorPool);
            socket.SetColor(colorNames[socketColorIdx], colors[socketColorIdx]);

            // 3️⃣ Create 1 correct shape (same color as the socket)
            CreateShape(socket.correctShape, colorNames[socketColorIdx]);

            // 4️⃣ Create incorrect shapes (for the remaining amount)
            for (int i = 0; i < shapesPerSocket - 1; i++)
            {
                string randomShape = shapeNames[Random.Range(0, shapeNames.Length)];
                
                // Pick unique colors for incorrect shapes from the pool too!
                int randomColorIdx = GetUniqueRandomIndex(availableColorPool);
                CreateShape(randomShape, colorNames[randomColorIdx]);
            }
        }
    }

    // 🌟 The core logic! Function to pick and remove without duplicates
    int GetUniqueRandomIndex(List<int> pool)
    {
        // If the pool is empty (e.g., more shapes than available colors), default to Red (index 0) to prevent out-of-bounds errors.
        if (pool.Count == 0) return 0; 

        // Pick a random index from the remaining pool.
        int randomPos = Random.Range(0, pool.Count);
        int pickedIndex = pool[randomPos];
        
        // 👈 Remove the picked color from the pool! (Core logic to prevent duplicates)
        pool.RemoveAt(randomPos); 
        
        return pickedIndex;
    }

    void CreateShape(string shape, string color)
    {
        GameObject prefab = GetPrefabByShape(shape);

        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        float padding = 0.5f; 

        // 👇 Narrow down the random Y-axis range to the middle of the screen!
        // For example, setting it between -1.5f and 1.5f spawns them near the center.
        Vector2 pos = new Vector2(
            Random.Range(-width + padding, width - padding), // X-axis (left/right) remains wide
            Random.Range(-1.5f, 1.5f)                        // Y-axis (up/down) constrained to the middle!
        );

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        DragObjectMobile drag = obj.GetComponent<DragObjectMobile>();
        drag.SetShapeAndColor(shape, color);
    }

    GameObject GetPrefabByShape(string shape)
    {
        if (shape == "Circle") return shapePrefabs[0];
        if (shape == "Square") return shapePrefabs[1];
        if (shape == "Triangle") return shapePrefabs[2];

        return shapePrefabs[0]; 
    }
}